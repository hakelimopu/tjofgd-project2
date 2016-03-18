open System

let rec mainMenu () :unit=
    ["";"Main Menu:";"";
     "[I]nitialization";
     "[Q]uit"]
    |> Console.writeLines

    let input = 
        [ConsoleKey.I; 
         ConsoleKey.Q]
        |> Set.ofList
        |> Console.filterInput

    match input with
    | ConsoleKey.I -> mainMenu |> Initialization.mainMenu
    | ConsoleKey.Q -> ()
    | _            -> raise (new NotImplementedException())


[<EntryPoint>]
let main argv = 
    mainMenu()
    0
