open System
open System.Numerics
open System.Drawing

open Svg

open LSystem
open LSystem.Systems.LineWalker
open LSystem.SVG

[<EntryPoint>]
let main argv =

    let p : LineWalkerParams =
        {
            StartPosition = Utils.makeVector2 1000.0f 1000.0f
            Line = Renderer.Primitives.makeLine Vector2.Zero (Utils.makeVector2 15.0f 0.0f);
            Theta = 90.0f |> Utils.radians;
        }
    
    let rules =
        let rng = System.Random()
        [
            LSystem.makeRule "F" "F+F-F-FF+F+F-F"
        ]

    let tokenise (p : LineWalkerParams) = function
        | 'F' -> [ Line p.Line ]
        | '-' -> [ Rotate p.Theta ]
        | '+' -> [ Rotate -p.Theta ]
        | _ as unmatched -> failwithf "Unrecognised character encountered %A" unmatched
    
    let lineWalker = LineWalker.make p rules tokenise 

    let input = "F+F+F+F"

    // generate until termination (iteration count or until rules fail to apply)
    let res = LSystem.evaluate input lineWalker.Rules 4

    printfn "%A" res
    //let counts = res |> Seq.countBy id 
    //printfn "%A" counts

    //let cmds = List.init res.Length (fun i -> Translator.translate res.[i])
    let cmds = Systems.LineWalker.LineWalker.makeRenderCommands lineWalker res

    let doc = Renderer.processCommands cmds

    doc.Write("cmd_result.svg")

    //let svg = SvgDocument()

    //let text = SvgText(res)
    //text.Color <- SvgColourServer(Color.Gainsboro)
    //text.FontSize <- SvgUnit(24.0f)
    //let xuc = SvgUnitCollection()
    //xuc.Add(SvgUnit(300.0f))
    //text.X <- xuc
    //let yuc = SvgUnitCollection()
    //yuc.Add(SvgUnit(300.0f))
    //text.Y <- yuc
    //svg.Children.Add(text)

    //let c = SvgCircle()
    //c.Radius <- SvgUnit(100.0f)
    //c.CenterX <- SvgUnit(100.0f) 
    //c.CenterY <- SvgUnit(100.0f) 
    //c.Fill <- SvgColourServer(Color.Gainsboro)
    //svg.Children.Add(c)

    //svg.Write("result.svg")
    let bitmap = doc.Draw()
    bitmap.Save("result.bmp")

    0
