module IslandMenuEncounterDetails

open GameState
open EncounterDetailUtilities

let private ``has visited island?`` (playState:PlayState<_>):bool =
    let ``is island that has been visited?`` (mapObject:MapObject.MapObject) :bool =
        match mapObject.Detail with
        | MapObject.Island props ->
            match props.Visits with
            | None -> false
            | Some x -> x > 0
        | _ -> false

    playState.Actors
    |> Map.exists (fun _ actor -> actor |> ``is island that has been visited?``)

let createIslandMenuEncounterDetail (playState:PlayState<_>) :EncounterDetail =
    let choices = 
        [({Text="Resume";    Response=Common Cancel}, ``always include choice``);
        ({Text="Map";    Response=Common Cancel}, ``always include choice``);
        ({Text="Information...";    Response=Common Cancel}, ``has visited island?``);
        ({Text="Head For...";    Response=Common Cancel}, ``has visited island?``)]
        |> List.filter (filterChoice playState)
        |> List.map fst

    let location = getBoatLocation playState

    {Location=location;
    Title="Island Menu";
    Type=EncounterType.Menu Island;
    Message=["Make your selection:"];
    Choices=choices;
    CurrentChoice=0} 


