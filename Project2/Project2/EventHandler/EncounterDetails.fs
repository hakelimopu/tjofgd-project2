module EncounterDetails

open CellLocation
open GameState


let createStorm (location:CellLocation) :EncounterDetail =
    {Location=location;
     Title="Storm!";
     Type=RanIntoStorm;
     Message=["You have run into a storm;";"it has damaged your boat!"];
     Choices=[{Text="OK";Response=Confirm}];
     CurrentChoice=0}

let createIsland (playState:PlayState) (location:CellLocation) :EncounterDetail =
    let canRepair (playState:PlayState) :bool =
        let boatProperties = playState |> getBoatProperties
        boatProperties.Hull < boatProperties.MaximumHull

    let choices: (EncounterChoice * (PlayState -> bool)) list = 
        [({Text="Cast Off!";   Response=Cancel}, fun state->true);
         ({Text="Repair Ship"; Response=Repair}, canRepair)]
        |> List.filter (fun (choice,func) -> playState |> func)

    let _,island =  getIsland location playState

    {Location=location;
    Title="Island!";
    Type=DockedWithIsland;
    Message=[island.Name |> sprintf "You docked at %s!";"What would you like to do?"];
    Choices=choices |> List.map fst;
    CurrentChoice=0} 
