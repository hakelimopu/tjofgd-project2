module QueryQuestEncounterChoice

open CellLocation
open GameState
open MapObject

let private acceptQuest (location:CellLocation) (playState:PlayState<_>) : PlayState<_> =
    let playerLocation, turn, boatProperties = 
        playState 
        |> getBoat

    let _, island = 
        playState 
        |> getIsland location

    let boatProperties' = 
        {boatProperties with Quest = island.Quest |> Some}

    let actors = 
        playState.Actors
        |> Map.add playerLocation {CurrentTurn = turn; Detail = boatProperties' |> Boat}

    //TODO: generate new quest for island.

    {playState with Encounters = None; Actors = actors}

let internal applyQueryQuestEncounterChoice (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | Confirm -> playState |> acceptQuest location |> PlayState |> Some
    | _ -> {playState with Encounters = None}|> PlayState |> Some


