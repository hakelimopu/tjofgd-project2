module BuySellEquipmentEncounterChoice

open CellLocation
open GameState
open Random
open BuySellEquipmentEncounterDetails
open MapObject
open EncounterChoiceUtilities

let private buyEquipment (location:CellLocation) (playState:PlayState<_>): PlayState<_> =
    {playState with Encounters = createBuyEquipmentEncounterDetail playState location |> PCEncounter |> Some}

let private sellEquipment (location:CellLocation) (playState:PlayState<_>): PlayState<_> =
    {playState with Encounters = createSellEquipmentEncounterDetail playState location |> PCEncounter |> Some}

let internal applyBuySellEquipmentEncounterChoice (randomFunc:RandomFunc) (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | EncounterReponse.Trade (TradeEncounterType.Equipment Buy)  -> playState |> buyEquipment detail.Location |> PlayState |> Some
    | EncounterReponse.Trade (TradeEncounterType.Equipment Sell) -> playState |> sellEquipment detail.Location |> PlayState |> Some
    | _                                       -> {playState with Encounters = None}|> PlayState |> Some

let purchaseEquipment (location:CellLocation) (equipmentType:EquipmentType) (playState:PlayState<_>) : PlayState<_> =
    let _ , island = getIsland location playState
    let boatProperties = getBoatProperties playState
    let equipment = equipmentTemplates.[equipmentType] |> snd
    let price = island.EquipmentPrices.[equipmentType]
    let boatProperties' = {boatProperties with Wallet = boatProperties.Wallet - price; Equipment = boatProperties.Equipment |> Seq.append [equipment]}
    playState
    |> setBoatProperties boatProperties'
    |> clearEncounters


let internal applyBuyEquipmentEncounterChoice (randomFunc:RandomFunc) (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | Purchase (PurchaseEncounterResponse.Equipment equipmentType) -> purchaseEquipment detail.Location equipmentType playState |> PlayState |> Some
    | _                                       -> {playState with Encounters = None}|> PlayState |> Some

let internal applySellEquipmentEncounterChoice (randomFunc:RandomFunc) (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | _                                       -> {playState with Encounters = None}|> PlayState |> Some


