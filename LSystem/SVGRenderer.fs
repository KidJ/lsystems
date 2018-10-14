namespace LSystem.SVG

open System
open System.Drawing
open System.Numerics

open LSystem

open Svg

module Renderer =

    module Primitives = 

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

        let rotateLine (theta : float32) (line : Line) =
            makeLine (Utils.rotate line.Start theta) (Utils.rotate line.End theta)

        let offsetLine (offset : Vector2) (line : Line)  =
            makeLine (line.Start + offset) (line.End + offset)

        let transformLine (offset : Vector2) (theta : float32) (line : Line)  =
            line |> rotateLine theta |> offsetLine offset

    open Primitives

    // Rendering primitives that will can be re-used by many different algorithms.
    type RenderCommand =
        | DrawLine of Line
        //| DrawRect of Rect

    let addLine (doc : SvgDocument byref) (line : Line) =

        let l = SvgLine()
        l.StartX <- SvgUnit(line.Start.X)
        l.StartY <- SvgUnit(line.Start.Y) 
        l.EndX <- SvgUnit(line.End.X)
        l.EndY <- SvgUnit(line.End.Y) 
        l.Stroke <- SvgColourServer(Color.Black)

        doc.Children.Add(l)
    
    let internal processCommand (doc : SvgDocument byref) = function
        | DrawLine line -> addLine &doc line
    
    let processCommands (commands : RenderCommand list) : SvgDocument =
    
        let mutable svg = SvgDocument()

        for c in commands do
            processCommand &svg c
        
        svg
    