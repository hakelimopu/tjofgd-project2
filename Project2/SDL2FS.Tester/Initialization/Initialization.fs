module Initialization

open System
open SDL

let initChoices = 
    [(Init.Init.Audio,          "Audio",          "[A]udio",          ConsoleKey.A);
     (Init.Init.Events,         "Events",         "[E]vents",         ConsoleKey.E);
     (Init.Init.GameController, "GameController", "[G]ameController", ConsoleKey.G);
     (Init.Init.Haptic,         "Haptic",         "[H]aptic",         ConsoleKey.H);
     (Init.Init.Joystick,       "Joystick",       "[J]oystick",       ConsoleKey.J);
     (Init.Init.Timer,          "Timer",          "[T]imer",          ConsoleKey.T);
     (Init.Init.Video,          "Video",          "[V]ideo",          ConsoleKey.V)]


let rec chooseFlags (initial:Set<Init.Init>) :Set<Init.Init> =
    ["";"Choose Initialization Flags:";""]
    |> Console.writeLines

    let processChoice (keys:Set<ConsoleKey>,keyMap:Map<ConsoleKey,Init.Init>) (flag:Init.Init,_,caption:string,key:ConsoleKey) :Set<ConsoleKey> * Map<ConsoleKey,Init.Init>=
        System.Console.Write caption
        if initial.Contains flag then
            System.Console.WriteLine "(on)"
        else
            System.Console.WriteLine "(off)"
        (keys.Add(key), keyMap |> Map.add key flag)

    let keys, keyMap = 
        ((Set.empty<ConsoleKey>, Map.empty<ConsoleKey,Init.Init>),initChoices)
        ||> List.fold processChoice

    Console.WriteLine("[D]one")

    let input = (keys.Add ConsoleKey.D) |> Console.filterInput
    if keyMap.ContainsKey input then
        let flag = keyMap.[input]
        if initial.Contains flag then
            initial 
            |> Set.remove flag
        else
            initial 
            |> Set.add flag
        |> chooseFlags
    else
        initial

let showInitFlags (system:SDL.Init.System) :unit=
    let processChoice (flag:Init.Init,name:string,_,_) :unit =
        Console.Write name
        if system.wasInit flag then
            Console.WriteLine "-on"
        else
            Console.WriteLine "-off"
        ()

    initChoices
    |> List.iter processChoice

let rec initMenu (returnMenu:unit->unit) :unit =
    let thisMenu () :unit= returnMenu |> initMenu
    let chosenFlags = Set.empty<Init.Init> |> chooseFlags
    let flagAccumulator (acc:Init.Init) (item:Init.Init) :Init.Init = acc ||| item
    let flags = 
        (Init.Init.None,chosenFlags)
        ||> Set.fold flagAccumulator
    let run () = 
        ["";"Initialization Results:";""]
        |> Console.writeLines
        use system = new SDL.Init.System(flags)
        system
        |> showInitFlags
    run()
    returnMenu()



let rec mainMenu (returnMenu:unit->unit) :unit =  
    let thisMenu () :unit= mainMenu returnMenu
    ["";"Initialization Menu:";"";
     "[I]nit";
     "[M]ain Menu"]
    |> Console.writeLines
    let input = 
        [ConsoleKey.I;
         ConsoleKey.M]
        |> Set.ofList
        |> Console.filterInput
    match input with
    | ConsoleKey.I -> thisMenu |> initMenu
    | ConsoleKey.M -> returnMenu()
    | _ -> raise (new NotImplementedException())
