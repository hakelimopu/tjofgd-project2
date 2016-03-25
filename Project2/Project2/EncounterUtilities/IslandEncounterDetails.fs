module IslandEncounterDetails

open CellLocation
open GameState
open EncounterDetailUtilities

let private ``can the ship repair?`` (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    boatProperties.Hull < boatProperties.MaximumHull

let private ``can accept quest?`` (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    boatProperties.Quest.IsNone

let createIslandEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =

    let choices = 
        [({Text="Cast Off!";   Response=Cancel},    ``always include choice``);
         ({Text="Repair Ship"; Response=Repair},    ``can the ship repair?``);
         ({Text="Need work!";  Response=QueryQuest},``can accept quest?``)]
        |> List.filter (filterChoice playState)
        |> List.map fst

    let _,island =  getIsland location playState

    {Location=location;
    Title="Island!";
    Type=DockedWithIsland;
    Message=[island.Name |> sprintf "You docked at %s!";"What would you like to do?"];
    Choices=choices;
    CurrentChoice=0} 
