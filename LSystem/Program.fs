open System
open System.Drawing

open Svg

open LSystem
open LSystem.SVG

module Translator = 

    let toRadians x = x * Math.PI / 180.0

    let translate = function
        | 'a' -> Renderer.DrawLine (Renderer.makeLine (Renderer.makeVector2 0.0f 0.0f) (Renderer.makeVector2 25.0f 0.0f))
        | 'b' -> Renderer.Rotate (toRadians 10.0)
        | 'c' -> Renderer.Rotate (toRadians -10.0)
        | 'd' -> Renderer.Rotate (toRadians 180.0)
        | _ as ruhroh -> failwithf "Invalid input \"%A\"!" ruhroh 


[<EntryPoint>]
let main argv =
    
    let rng = System.Random()

    let rules = 
        [
            //LSystem.makeRule "a" "ab";
            LSystem.makeWeightedRandomRule "a" [ (20.0, "b"); (2.0, "c"); (1.0, "d") ] rng
            //LSystem.makeWeightedRandomRule "b" [ (1.0, "b"); (1.0, "d") ] rng
            //LSystem.makeRule "ababa" "c";
            //LSystem.makeRule "c" "caaa";
            //LSystem.makeRule "aaaa" "aaac";
        ]

    let input = String.replicate 1000 "a"

    // generate until termination (iteration count or until rules fail to apply)
    let res = LSystem.evaluate input rules 1

    printfn "%A" res
    //let counts = res |> Seq.countBy id 
    //printfn "%A" counts

    let cmds = List.init res.Length (fun i -> Translator.translate res.[i])

    let doc = Renderer.processCommands cmds

    doc.Write("cmd_result.svg")

    // Example of SVG lib usage
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
    //let bitmap = svg.Draw()
    //bitmap.Save("result.bmp")

    0
