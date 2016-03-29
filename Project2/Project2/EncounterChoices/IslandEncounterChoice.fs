﻿module IslandEncounterChoice

open GameState
open CellLocation
open QueryQuestEncounterDetails
open MapObject
open QueryRepairEncounterDetails

let private queryRepair (location:CellLocation) (playState:PlayState<_>) : PlayState<_> =
    let encounter = 
        location
        |> createQueryRepairEncounterDetail playState
        |> PCEncounter
        |> Some
    {playState with Encounters = encounter}

let private queryQuest (location:CellLocation) (playState:PlayState<_>) : PlayState<_> =
    let encounter = 
        location
        |> createQuestQueryEncounterDetail playState
        |> PCEncounter
        |> Some
    {playState with Encounters = encounter}

let private completeQuest (playState:PlayState<_>) : PlayState<_> =
    let location, turn, boatProperties = getBoat playState

    let quest = boatProperties.Quest |> Option.get

    let boatProperties' =
        {boatProperties with Quest = None; Wallet = boatProperties.Wallet + quest.Reward}

    let actors =
        playState.Actors
        |> Map.add location {CurrentTurn = turn; Detail = (boatProperties' |> Boat)}

    {playState with Encounters=None; Actors = actors}

let internal applyIslandPCEncounterChoice (detail:EncounterDetail) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | Repair -> playState |> queryRepair detail.Location |> PlayState |> Some
    | QueryQuest -> playState |> queryQuest detail.Location |> PlayState |> Some
    | CompleteQuest ->  playState |> completeQuest |> PlayState |> Some
    | _ -> {playState with Encounters=None} |> PlayState |> Some
    
