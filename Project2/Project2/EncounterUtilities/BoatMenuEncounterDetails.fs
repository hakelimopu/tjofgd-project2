module BoatMenuEncounterDetails

open GameState
open EncounterDetailUtilities

let ``has equipment?`` = BuySellEquipmentEncounterDetails.``can sell equipment?``

let private ``has quest?`` (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    boatProperties.Quest.IsSome


let createBoatMenuEncounterDetail (playState:PlayState<_>) :EncounterDetail =
    let choices = 
        [({Text="Resume";    Response=Common Cancel}, ``always include choice``);
        ({Text="Equipment";    Response=Common Cancel}, ``has equipment?``);
        ({Text="Statistics";    Response=Common Cancel}, ``always include choice``);
        ({Text="Quest";    Response=Common Cancel}, ``has quest?``)]
        |> List.filter (filterChoice playState)
        |> List.map fst

    let location = getBoatLocation playState

    {Location=location;
    Title="Boat Menu";
    Type=EncounterType.Menu MenuType.Boat;
    Message=["Make your selection:"];
    Choices=choices;
    CurrentChoice=0} 
