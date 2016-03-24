module EncounterDetails

open CellLocation
open GameState


let createStormEncounterDetail (location:CellLocation) :EncounterDetail =
    {Location=location;
     Title="Storm!";
     Type=RanIntoStorm;
     Message=["You have run into a storm;";"it has damaged your boat!"];
     Choices=[{Text="OK";Response=Confirm}];
     CurrentChoice=0}

let private ``can the ship repair?`` (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    boatProperties.Hull < boatProperties.MaximumHull

let private ``always include choice`` (playState:PlayState<_>) :bool = true

let private filterChoice (playState:PlayState<_>) (choice, func) =
    playState |> func

let createIslandEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =

    let choices = 
        [({Text="Cast Off!";   Response=Cancel}, ``always include choice``);
         ({Text="Repair Ship"; Response=Repair}, ``can the ship repair?``)]
        |> List.filter (filterChoice playState)
        |> List.map fst

    let _,island =  getIsland location playState

    {Location=location;
    Title="Island!";
    Type=DockedWithIsland;
    Message=[island.Name |> sprintf "You docked at %s!";"What would you like to do?"];
    Choices=choices;
    CurrentChoice=0} 
