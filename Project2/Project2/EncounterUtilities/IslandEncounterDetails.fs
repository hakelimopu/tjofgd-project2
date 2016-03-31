﻿module IslandEncounterDetails

open CellLocation
open GameState
open EncounterDetailUtilities
open MapObject

let private ``can the ship repair?`` (location:CellLocation) (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    let damage = boatProperties.MaximumHull - boatProperties.Hull
    if damage > 0<health> then
        let _, island = getIsland location playState
        boatProperties.Wallet >= island.RepairCost * 1.0<health>
    else
        false

let private ``can accept quest?`` (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    boatProperties.Quest.IsNone

let private ``is quest complete?`` (location:CellLocation) (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    match boatProperties.Quest with
    | Some quest -> quest.Destination = location
    | _          -> false

let createIslandEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =

    let choices = 
        [({Text="Cast Off!";   Response=Cancel},        ``always include choice``);
         ({Text="Repair Ship"; Response=Repair},        ``can the ship repair?`` location);
         ({Text="Need work!";  Response=QueryQuest},    ``can accept quest?``);
         ({Text="Delivery!";   Response=CompleteQuest}, ``is quest complete?`` location)]
        |> List.filter (filterChoice playState)
        |> List.map fst

    let _,island =  getIsland location playState

    {Location=location;
    Title="Island!";
    Type=DockedWithIsland;
    Message=
        [island.Name |> sprintf "You docked at %s!";
        island.Visits |> sprintf "Prior visits: %d";
        "What would you like to do?"];
    Choices=choices;
    CurrentChoice=0} 
