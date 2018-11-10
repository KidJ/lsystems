namespace LSystem.Render

open System
open System.Drawing
open System.Numerics

open LSystem
open LSystem.Systems
open Geometry
open Svg

module SVGRenderer =

    // Rendering primitives that will can be re-used by many different algorithms.
    // E.g. draw rect, draw circle, shading, etc.
    let addLine (doc : SvgDocument byref) (line : Line) =

        let l = SvgLine()
        l.StartX <- SvgUnit(line.Start.X)
        l.StartY <- SvgUnit(line.Start.Y) 
        l.EndX <- SvgUnit(line.End.X)
        l.EndY <- SvgUnit(line.End.Y) 
        l.Stroke <- SvgColourServer(Color.Black)

        doc.Children.Add(l)
    
    let internal processCommand (doc : SvgDocument byref) = function
        | LineWalker.DrawLine line -> addLine &doc line
    
    let processCommands (commands : LineWalker.RenderCommand list) : SvgDocument =
    
        let mutable svg = SvgDocument()

        for c in commands do
            processCommand &svg c
        
        svg
    