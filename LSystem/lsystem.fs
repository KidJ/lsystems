namespace LSystem

open System

module LSystem =

    type Rule =
        {
            Expr : string
            ResultExpr : unit -> string
        }

    let makeRule expr result =
        {
            Expr = expr;
            ResultExpr = fun () -> result;
        }

    // Creates a function which selects from arguments according to the weights provided.
    let weightedRandom (a : (float * 'a) list) =
        let weights, values = a |> List.unzip
        let valuesa = values |> Array.ofList
        let cumprob =
            let sum = a |> List.sumBy fst
            weights
            |> List.scan (fun acc w -> w + acc) 0.0
            |> List.tail
            |> List.map (fun w -> w / sum)
            |> Array.ofList
        
        fun (r : float) ->
            match cumprob |> Array.tryFindIndex (fun v ->  v >= r) with
            | Some i -> valuesa.[i]
            | None -> valuesa.[valuesa.Length - 1]

    let makeWeightedRandomRule (expr : string) (results : (float * string) list) (rng : System.Random) : Rule =
        let f = weightedRandom results
        {
            Expr = expr;
            ResultExpr = fun () -> f (rng.NextDouble())
        }

    type RuleSet = 
        {
            Rules : Rule list
            Selector : Rule list -> Rule option // given a list of matching rules, which one to select
        }

    //module RuleSet =
        
    //    let matchingRules (set : RuleSet) (input : string) : Rule option =
    //        failwithf "todo"

    //    let selectShortest (rules : Rule list) =
    //        match rules with
    //        | a when a = List.empty -> None
    //        | a -> Some (rules |> List.sortBy (fun r -> r.Expr.Length) |> List.head)

    //    let selectLongest (rules : Rule list) =
    //        match rules with
    //        | a when a = List.empty -> None
    //        | a -> Some (rules |> List.sortByDescending (fun r -> r.Expr.Length) |> List.head)

    //    let selectRandom (seed : int option) : Rule list -> Rule option =
    //        let rng = System.Random(Option.defaultValue (DateTime.Now.Ticks |> int) seed)
    //        fun (rules : Rule list)  ->
    //            match rules with
    //            | a when a = List.empty -> None
    //            | a -> Some (rules |> List.item (rng.Next(0, List.length rules)))

    let rec step (input : string) (rules : Rule list) : string =
        if input.Length > 0 then
            // match all rules against input string
            let matchingRules = rules |> List.filter (fun rule -> rule.Expr.Length <= input.Length && rule.Expr = input.Substring(0,rule.Expr.Length))

            if not <| List.isEmpty matchingRules then
                // select matching rule from those with greatest length
                let r = matchingRules |> List.sortByDescending (fun rule -> rule.Expr.Length) |> List.head
                let remainder = input.Substring(r.Expr.Length)
                r.ResultExpr() + (step remainder rules)
            else
                // *shudders*
                //input.[0..1] + (step (input.[1..]) rules) //  / input.Substring(1)
                input.Substring(0,1) + (step (input.Substring(1)) rules) // input.Substring(0,1)  / input.Substring(1)
        else
            ""

    let rec evaluateInner (input : string) (rules : Rule list) (iterations : int) : string list =
        if iterations > 0 then
            let stepResult = step input rules
            stepResult :: (evaluateInner stepResult rules (iterations - 1))
        else
            List.empty

    let evaluate (input : string) (rules : Rule list) (iterations : int) : string =
        if iterations < 1 then failwithf "Must have at least one iteration"

        let results = evaluateInner input rules iterations

        results |> List.iteri (fun i str -> printfn "%d : %s" i str)

        results |> List.rev |> List.head
        