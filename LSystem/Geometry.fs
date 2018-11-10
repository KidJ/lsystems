namespace LSystem

open LSystem
open System.Numerics

module Geometry =

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

    type Rect =
        {
            Position : Vector2
            Width : float32
            Height : float32
        }
