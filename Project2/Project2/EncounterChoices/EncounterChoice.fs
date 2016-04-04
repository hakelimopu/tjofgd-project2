module EncounterChoice

open GameState
open MapCell
open MapObject
open CellLocation
open Random
open QueryQuestEncounterDetails
open EncounterChoiceUtilities
open StormEncounterChoice
open IslandEncounterChoice
open QueryQuestEncounterChoice
open QueryRepairEncounterChoice
open BuySellEncounterChoice

let private applyPCEncounterChoice (randomFunc:RandomFunc) (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (details:EncounterDetail) (playState:PlayState<_>) : GameState<_> option =
    match details.Type with
    | RanIntoStorm             -> applyStormEncounterChoice sumLocationsFunc setVisibleFunc details.Location true None playState
    | DockedWithIsland         -> applyIslandPCEncounterChoice details playState
    | EncounterType.QueryQuest -> applyQueryQuestEncounterChoice randomFunc details details.Location playState
    | EncounterType.QueryRepair -> applyQueryRepairEncounterChoice randomFunc details details.Location playState
    | EncounterType.BuySellEquipment -> applyBuySellEquipmentEncounterChoice randomFunc details details.Location playState

let private applyNPCEncounterChoice (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (head:EncounterDetail) (tail:EncounterDetail list) (playState:PlayState<_>): GameState<_> option =
    let nextEncounter =
        match tail with
        | [] -> None
        | _ -> tail |> NPCEncounters |> Some
    match head.Type with 
    | RanIntoStorm -> 
        applyStormEncounterChoice sumLocationsFunc setVisibleFunc head.Location false nextEncounter playState
    | _ -> raise (new System.NotImplementedException("This encounter is not implemented for NPC encounters!"))

let internal applyEncounterChoice (randomFunc:RandomFunc) (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (playState:PlayState<_>) :GameState<_> option =
    match playState.Encounters with
    | Some (PCEncounter details) -> 
        (details, playState) ||> applyPCEncounterChoice randomFunc sumLocationsFunc setVisibleFunc
    | Some (NPCEncounters (head::tail)) -> 
        (head, tail, playState) |||> applyNPCEncounterChoice sumLocationsFunc setVisibleFunc
    | _ -> playState |> PlayState |> Some



