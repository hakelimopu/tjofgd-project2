﻿module EncounterChoice

open GameState
open MapCell
open CellLocation
open Random
open StormEncounterChoice
open IslandEncounterChoice
open QueryQuestEncounterChoice
open QueryRepairEncounterChoice
open BuySellEquipmentEncounterChoice
open MainMenuEncounterChoice
open GameMenuEncounterChoice

let private applyPCEncounterChoice (randomFunc:RandomFunc) (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (details:EncounterDetail) (playState:PlayState<_>) : GameState<_> option =
    match details.Type with
    | RanIntoStorm                                  -> applyStormEncounterChoice sumLocationsFunc setVisibleFunc details.Location true None playState
    | DockedWithIsland                              -> applyIslandPCEncounterChoice details playState
    | EncounterType.Menu Main                       -> applyMainMenuPCEncounterChoice details playState
    | EncounterType.Menu Game                       -> applyGameMenuPCEncounterChoice details playState
    | EncounterType.Query QueryEncounterType.Quest  -> applyQueryQuestEncounterChoice randomFunc details details.Location playState
    | EncounterType.Query QueryEncounterType.Repair -> applyQueryRepairEncounterChoice randomFunc details details.Location playState
    | EncounterType.Trade (TradeEncounterType.Equipment BuyOrSell)     -> applyBuySellEquipmentEncounterChoice randomFunc details details.Location playState
    | EncounterType.Trade (TradeEncounterType.Equipment Buy)           -> applyBuyEquipmentEncounterChoice randomFunc details details.Location playState
    | EncounterType.Trade (TradeEncounterType.Equipment Sell)          -> applySellEquipmentEncounterChoice randomFunc details details.Location playState
    | _                                             -> playState |> PlayState |> Some

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



