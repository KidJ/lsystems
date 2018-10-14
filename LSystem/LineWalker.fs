namespace LSystem.Systems

open LSystem
open LSystem.SVG

open System.Numerics

// Walks through space drawing lines with random rotations.
module LineWalker =

    let rules =
        let rng = System.Random()
        [
            LSystem.makeWeightedRandomRule "a" [ (1.0, "aa"); (1.0, "ab"); (1.0, "ac") ] rng
            LSystem.makeWeightedRandomRule "b" [ (1.0, "b"); (2.0, "ba") ] rng
            LSystem.makeWeightedRandomRule "c" [ (1.0, "c"); (2.0, "ca") ] rng
        ]
    
    type Token =
        | Line of Renderer.Primitives.Line
        | Rotate of float32
        | Move of Vector2

    open Renderer

    let lineLength = 20.0f
    let rotation = 25.0f

    let defaultLine =
        Primitives.makeLine Vector2.Zero (Utils.makeVector2 50.0f 0.0f)

    let internal tokenise = function
        | 'a' -> [ Line <| defaultLine ]
        | 'b' -> [ Rotate rotation ]
        | 'c' -> [ Rotate -rotation ]
        | _ as unmatched -> failwithf "Unrecognised character encountered %A" unmatched

    
    type internal State =
        {
            Position : Vector2
            Rotation : float32
            Commands : RenderCommand list
        }

    let internal startingState =
        {
            Position = Utils.makeVector2 25.0f 250.0f;
            Rotation = 0.0f;
            Commands = []
        }

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
        | Move offset -> { state with Position = state.Position + (Utils.rotate offset (Utils.radians state.Rotation)) }

    let makeRenderCommands (stream : string) : Renderer.RenderCommand list =
        let tokenised = stream |> Seq.map tokenise |> Seq.concat |> List.ofSeq
        let states = tokenised |> List.scan (processToken) startingState

        // as we append most recent command as head, finally reverse list!
        List.rev <| (List.last states).Commands
