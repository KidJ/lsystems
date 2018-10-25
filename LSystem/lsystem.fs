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

    let step (input : string) (rules : Rule list) : string =
        let builder = System.Text.StringBuilder()
        let mutable start = 0
        while start < input.Length do
            // match all rules against input string
            let remainder = input.Length - start
            let matchingRules = rules |> List.filter (fun rule -> rule.Expr.Length <= remainder && rule.Expr = input.Substring(start,rule.Expr.Length))

            if not <| List.isEmpty matchingRules then
                // select matching rule with greatest length
                let r = matchingRules |> List.sortByDescending (fun rule -> rule.Expr.Length) |> List.head
                builder.Append(r.ResultExpr()) |> ignore
                start <- start + r.Expr.Length
            else
                builder.Append(input.[start]) |> ignore
                start <- start + 1
                

        string builder

    let rec evaluateInner (input : string) (rules : Rule list) (iterations : int) : string list =
        if iterations > 0 then
            let stepResult = step input rules
            stepResult :: (evaluateInner stepResult rules (iterations - 1))
        else
            List.empty

    let evaluate (input : string) (rules : Rule list) (iterations : int) : string =
        if iterations < 1 then failwithf "Must have at least one iteration"

        let results = evaluateInner input rules iterations

        results |> List.rev |> List.head
        