module QueryQuestEncounterChoice

open CellLocation
open GameState
open MapObject
open MapCreate
open Random

let private acceptQuest (randomFunc:RandomFunc) (location:CellLocation) (playState:PlayState<_>) : PlayState<_> =
    let playerLocation, turn, boatProperties = 
        playState 
        |> getBoat

    let islandTurn, island = 
        playState 
        |> getIsland location

    let boatProperties' = 
        {boatProperties with Quest = island.Quest |> Some; BoundFor = location |> Some}

    let islands,_ = 
        playState.Actors
        |> Map.partition (fun k v -> 
            match Some v with 
            | IsIsland -> k <> location
            | _ -> false)

    let location' =
        islands
        |> Map.toSeq
        |> Seq.map (fun (k,v)->k)
        |> Seq.minBy (fun e->NextInt |> randomFunc |> getInt)

    let quest' = generateQuest randomFunc location'

    let island' = {island with Quest = quest'}

    let actors = 
        playState.Actors
        |> Map.add playerLocation {CurrentTurn = turn; Detail = boatProperties' |> Boat}
        |> Map.add location {CurrentTurn = islandTurn; Detail = island' |> Island}

    {playState with Encounters = None; Actors = actors}

let internal applyQueryQuestEncounterChoice (randomFunc:RandomFunc) (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | Common Confirm -> playState |> acceptQuest randomFunc location |> PlayState |> Some
    | _ -> {playState with Encounters = None}|> PlayState |> Some


