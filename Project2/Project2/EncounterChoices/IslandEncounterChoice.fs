module IslandEncounterChoice

open GameState
open CellLocation
open QueryQuestEncounterDetails
open MapObject
open QueryRepairEncounterDetails
open BuySellEncounterDetails

let private generateEncounterDetail (encounterDetailGenerator:PlayState<_>->CellLocation->EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : PlayState<_> =
    let encounter = 
        location
        |> encounterDetailGenerator playState
        |> PCEncounter
        |> Some
    {playState with Encounters = encounter}

let private queryRepair (location:CellLocation) (playState:PlayState<_>) : PlayState<_> =
    generateEncounterDetail createQueryRepairEncounterDetail location playState

let private queryQuest (location:CellLocation) (playState:PlayState<_>) : PlayState<_> =
    generateEncounterDetail createQuestQueryEncounterDetail location playState

let private buySellEquipment  (location:CellLocation) (playState:PlayState<_>) : PlayState<_> =
    generateEncounterDetail createBuySellEncounterDetail location playState

let private completeQuest (playState:PlayState<_>) : PlayState<_> =
    let location, turn, boatProperties = getBoat playState

    let quest = boatProperties.Quest |> Option.get

    let boatProperties' =
        {boatProperties with Quest = None; Wallet = boatProperties.Wallet + quest.Reward}

    let actors =
        playState.Actors
        |> Map.add location {CurrentTurn = turn; Detail = (boatProperties' |> Boat)}

    {playState with Encounters=None; Actors = actors}

let private incrementIslandVisit (location:CellLocation) (playState:PlayState<_>) :PlayState<_> =
    let turn, island = getIsland location playState

    let island' =
        {island with Visits = if island.Visits.IsSome then (island.Visits.Value + 1) |> Some else Some 1}

    let actors' = 
        playState.Actors
        |> Map.add location {CurrentTurn = turn; Detail = island' |> Island}

    {playState with Actors = actors'}

let internal applyIslandPCEncounterChoice (detail:EncounterDetail) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | Repair           -> playState |> queryRepair detail.Location      |> incrementIslandVisit detail.Location |> PlayState |> Some
    | QueryQuest       -> playState |> queryQuest  detail.Location      |> incrementIslandVisit detail.Location |> PlayState |> Some
    | CompleteQuest    -> playState |> completeQuest                    |> incrementIslandVisit detail.Location |> PlayState |> Some
    | BuySellEquipment -> playState |> buySellEquipment detail.Location |> incrementIslandVisit detail.Location |> PlayState |> Some
    | _                -> {playState with Encounters=None}              |> incrementIslandVisit detail.Location |> PlayState |> Some
    
