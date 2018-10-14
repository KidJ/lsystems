namespace LSystem.SVG

open System
open System.Drawing

open Svg

module Renderer =
    
    type Vector2 =
        {
            X : float32
            Y : float32
        }
        with static member (+) (a,b) = { X = a.X + b.X; Y = a.Y + b.Y }

    let makeVector2 x y =
        {
            X = x;
            Y = y;
        }

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
        | Rotate of float
        | Move of Vector2
    
    type RenderState =
        {
            mutable Position : Vector2
            mutable Rotation : float
        }

    let addLine (doc : SvgDocument byref) (line : Line) =

        let l = SvgLine()
        l.StartX <- SvgUnit(line.Start.X)
        l.StartY <- SvgUnit(line.Start.Y) 
        l.EndX <- SvgUnit(line.End.X)
        l.EndY<- SvgUnit(line.End.Y) 
        l.Stroke <- SvgColourServer(Color.Black)

        doc.Children.Add(l)

    let rotate2d (v : Vector2) (theta : float) =
        let s = float32 <| Math.Sin(theta)
        let c = float32 <| Math.Cos(theta)
        {
            X = v.X * c - v.Y * s
            Y = v.X * s - v.Y * c
        }

    let rotateLine (theta : float) (line : Line) =
        makeLine (rotate2d line.Start theta) (rotate2d line.End theta)

    let offsetLine (offset : Vector2) (line : Line)  =
        makeLine (line.Start + offset) (line.End + offset)

    let transformLine (offset : Vector2) (theta : float) (line : Line)  =
        line |> rotateLine theta |> offsetLine offset

    let internal processCommand (doc : SvgDocument byref) (state : RenderState byref) = function
        | DrawLine line ->
            let line' = (line |> transformLine state.Position state.Rotation)
            addLine &doc line'
            state.Position <- line'.End
        | Rotate theta -> state.Rotation <- state.Rotation + theta
        | Move offset -> state.Position <- state.Position + (rotate2d offset state.Rotation)

    let processCommands (commands : RenderCommand list) : SvgDocument =
    
        let mutable svg = SvgDocument()
        let mutable renderState = { Position = makeVector2 0.0f 250.0f; Rotation = 0.0 }

        for c in commands do
            processCommand &svg &renderState c
        
        svg
