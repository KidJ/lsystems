namespace LSystem.Systems

open LSystem
open LSystem.SVG

open System.Numerics

type LineWalkerParams =
    {
        StartPosition : Vector2
        Line : Renderer.Primitives.Line
        Theta : float32
    }

type LineWalker = 
    {
        Params : LineWalkerParams
        Rules : LSystem.Rule list
    }

// Walks through space drawing lines with random rotations.
module LineWalker =

    open Renderer

    type internal State =
        {
            Position : Vector2
            Rotation : float32
            Commands : RenderCommand list
        }

    let internal makeInitialState (lineWalker : LineWalker) : State =
        {
            Position = lineWalker.Params.StartPosition;
            Rotation = lineWalker.Params.Theta;
            Commands = [];
        }

    let rules =
        let rng = System.Random()
        [
            LSystem.makeWeightedRandomRule "a" [ (1.0, "aa"); (1.0, "ab"); (1.0, "ac") ] rng
            LSystem.makeWeightedRandomRule "b" [ (1.0, "b"); (2.0, "ba") ] rng
            LSystem.makeWeightedRandomRule "c" [ (1.0, "c"); (2.0, "ca") ] rng
        ]

    let make (p : LineWalkerParams) : LineWalker =
        {
            Params = p;
            Rules = rules;
        }
    
    type Token =
        | Line of Renderer.Primitives.Line
        | Rotate of float32
        | Move of Vector2

    let internal tokenise (p : LineWalkerParams) = function
        | 'a' -> [ Line <| p.Line ]
        | 'b' -> [ Rotate p.Theta ]
        | 'c' -> [ Rotate -p.Theta ]
        | _ as unmatched -> failwithf "Unrecognised character encountered %A" unmatched

    let internal processToken (state : State) (tok : Token) : State =
        match tok with 
        | Line l ->
            let l' = (l |> Renderer.Primitives.transformLine state.Position state.Rotation)
            {
                Position = l'.End
                Rotation = state.Rotation
                Commands = (DrawLine l') :: state.Commands
            }
        | Rotate theta -> { state with Rotation = state.Rotation + theta }
        | Move offset -> { state with Position = state.Position + (Utils.rotate offset state.Rotation) }

    let makeRenderCommands (lineWalker : LineWalker) (stream : string) : Renderer.RenderCommand list =
        let tokenised = stream |> Seq.map (tokenise lineWalker.Params) |> Seq.concat |> List.ofSeq
        let state = makeInitialState lineWalker
        let states = tokenised |> List.scan (processToken) state

        // as we append most recent command as head, finally reverse list!
        List.rev <| (List.last states).Commands
