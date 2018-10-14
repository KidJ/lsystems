namespace LSystem.SVG

open System
open System.Drawing
open System.Numerics

open LSystem

open Svg

module Renderer =

    type Line = 
        {
            Start : Vector2
            End  : Vector2
        }

    let makeLine s e =
        {
            Start = s;
            End = e;
        }

    // Rendering primitives that will can be re-used by many different algorithms.
    type RenderCommand =
        | DrawLine of Line
        | Rotate of float32
        | Move of Vector2
    
    type RenderState =
        {
            mutable Position : Vector2
            mutable Rotation : float32
        }

    let addLine (doc : SvgDocument byref) (line : Line) =

        let l = SvgLine()
        l.StartX <- SvgUnit(line.Start.X)
        l.StartY <- SvgUnit(line.Start.Y) 
        l.EndX <- SvgUnit(line.End.X)
        l.EndY<- SvgUnit(line.End.Y) 
        l.Stroke <- SvgColourServer(Color.Black)

        doc.Children.Add(l)

    let rotateLine (theta : float32) (line : Line) =
        makeLine (Utils.rotate line.Start theta) (Utils.rotate line.End theta)

    let offsetLine (offset : Vector2) (line : Line)  =
        makeLine (line.Start + offset) (line.End + offset)

    let transformLine (offset : Vector2) (theta : float32) (line : Line)  =
        line |> rotateLine theta |> offsetLine offset

    let internal processCommand (doc : SvgDocument byref) (state : RenderState byref) = function
        | DrawLine line ->
            let line' = (line |> transformLine state.Position state.Rotation)
            addLine &doc line'
            state.Position <- line'.End
        | Rotate theta -> state.Rotation <- state.Rotation + theta
        | Move offset -> state.Position <- state.Position + (Utils.rotate offset state.Rotation)

    let processCommands (commands : RenderCommand list) : SvgDocument =
    
        let mutable svg = SvgDocument()
        let mutable renderState = { Position = Utils.makeVector2 0.0f 250.0f; Rotation = 0.0f }

        for c in commands do
            processCommand &svg &renderState c
        
        svg
