﻿namespace LSystem.Systems

open LSystem
open System.Numerics

// Walks through space drawing lines with random rotations.
module LineWalker =

    type Token =
        | Line of Geometry.Line
        | Rotate of float32
        | Move of Vector2

    type LineWalkerParams =
        {
            StartPosition : Vector2
            Line : Geometry.Line
            Theta : float32
        }

    type RenderCommand =
        | DrawLine of Geometry.Line
        //| DrawRect of Rect

    type LineWalker = 
        {
            Params : LineWalkerParams
            Rules : LSystem.Rule list
            Tokeniser : LineWalkerParams -> char -> Token list
        }
    
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

    let make (p : LineWalkerParams) (rules : LSystem.Rule List) (tokeniser : LineWalkerParams -> char -> Token list) : LineWalker =
        {
            Params = p;
            Rules = rules;
            Tokeniser = tokeniser;
        }

    let internal processToken (state : State) (tok : Token) : State =
        match tok with 
        | Line l ->
            let l' = (l |> Geometry.transformLine state.Position state.Rotation)
            {
                Position = l'.End
                Rotation = state.Rotation
                Commands = (DrawLine l') :: state.Commands
            }
        | Rotate theta -> { state with Rotation = state.Rotation + theta }
        | Move offset -> { state with Position = state.Position + (Utils.rotate offset state.Rotation) }

    let makeRenderCommands (lineWalker : LineWalker) (stream : string) : RenderCommand list =
        let tokenised = stream |> Seq.map (lineWalker.Tokeniser lineWalker.Params) |> Seq.concat |> List.ofSeq
        let state = makeInitialState lineWalker
        let states = tokenised |> List.scan (processToken) state

        // as we append most recent command as head, finally reverse list!
        List.rev <| (List.last states).Commands
