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
    | EncounterResponse.Trade (TradeEncounterType.Equipment Buy)  -> playState |> buyEquipment detail.Location |> PlayState |> Some
    | EncounterResponse.Trade (TradeEncounterType.Equipment Sell) -> playState |> sellEquipment detail.Location |> PlayState |> Some
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
    | _                                                            -> {playState with Encounters = None}|> PlayState |> Some

let internal sellEquipmentItem (location: CellLocation) (index:int) (playState:PlayState<_>) : PlayState<_> =
    let _ , island = getIsland location playState
    let boatProperties = getBoatProperties playState
    let equipmentSold = boatProperties.Equipment |> Seq.item index
    let remainingEquipment = 
        ((Seq.empty,0), boatProperties.Equipment)
        ||> Seq.fold(fun (equipment',index') equipmentDetail ->
            if index'=index then
                (equipment',index'+1)
            else
                (equipment' |> Seq.append [equipmentDetail],index'+1))
        |> fst
    let equipmentType = equipmentSold.Type
    let basePrice = island.EquipmentPrices.[equipmentType]
    let fraction = equipmentSold.Durability / equipmentSold.MaximumDurability
    let fairMarketValue = basePrice * fraction / 2.0
    let boatProperties' =
        {boatProperties with Equipment = remainingEquipment; Wallet = boatProperties.Wallet + fairMarketValue}
    playState
    |> setBoatProperties boatProperties'
    |> clearEncounters
    

let internal applySellEquipmentEncounterChoice (randomFunc:RandomFunc) (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | Sale (Equipment index) -> sellEquipmentItem detail.Location index playState |> PlayState |> Some
    | _                      -> {playState with Encounters = None}|> PlayState |> Some


