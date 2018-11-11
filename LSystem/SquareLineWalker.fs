namespace LSystem.Systems.Examples

open System.Numerics

open LSystem
open LSystem.Systems

module SquareLineWalker =

    let parameters : LineWalker.LineWalkerParams =
        {
            StartPosition = Utils.makeVector2 0.0f 0.0f
            Line = Geometry.makeLine Vector2.Zero (Utils.makeVector2 10.0f 0.0f);
            Theta = 90.0f |> Utils.radians;
        }
    
    let rules =
            [
                LSystem.makeRule "F" "F+F-F-FF+F+F-F"
            ]

    let tokenise (p : LineWalker.LineWalkerParams) = function
        | 'F' -> [ LineWalker.Line p.Line ]
        | '-' -> [ LineWalker.Rotate p.Theta ]
        | '+' -> [ LineWalker.Rotate -p.Theta ]
        | _ as unmatched -> failwithf "Unrecognised character encountered %A" unmatched

    let make () = 
        LineWalker.make parameters rules tokenise
