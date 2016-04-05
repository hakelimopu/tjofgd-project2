module BuySellEquipmentEncounterDetails

open GameState
open CellLocation
open MapObject
open EncounterDetailUtilities

let internal ``can buy equipment?`` (location:CellLocation) (playState:PlayState<_>) :bool =
    let boatProps = getBoatProperties playState
    let _, island =  getIsland location playState

    island.EquipmentPrices
    |> Map.exists (fun k v -> v <= boatProps.Wallet)

let internal ``can sell equipment?`` (playState:PlayState<_>) :bool =
    let boatProps = getBoatProperties playState
    boatProps.Equipment
    |> Seq.isEmpty
    |> not

let createBuySellEquipmentEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =
    let choices = 
        [({Text="Never Mind!";    Response=Common Cancel},        ``always include choice``);
         ({Text="Buy Equipment";  Response=EncounterReponse.Trade (TradeEncounterType.Equipment Buy)},  ``can buy equipment?`` location);
         ({Text="Sell Equipment"; Response=EncounterReponse.Trade (TradeEncounterType.Equipment Sell)}, ``can sell equipment?``)]
        |> List.filter (filterChoice playState)
        |> List.map fst

    {Location=location;
    Title="Buy/Sell Equipment";
    Type=EncounterType.Trade (TradeEncounterType.Equipment BuyOrSell);
    Message=["What would you like to do?"];
    Choices=choices;
    CurrentChoice=0} 

let createBuyEquipmentEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =
    let choices = 
        [{Text="Never Mind!"; Response=Common Cancel}]

    let _, island =  getIsland location playState

    let boatProperties = getBoatProperties playState

    let choices' =
        (choices, island.EquipmentPrices)
        ||> Map.fold (fun choices equipmentType price -> 
            if price<= boatProperties.Wallet then
                let name = 
                    equipmentTemplates.[equipmentType]
                    |> fst
                [{Text=(name,price) ||> sprintf "%s($%.2f)";Response = Purchase (PurchaseEncounterResponse.Equipment equipmentType)}]
                |> List.append choices
            else
                choices)

    {Location=location;
    Title="Buy Equipment";
    Type=EncounterType.Trade (TradeEncounterType.Equipment Buy);
    Message=["What would you like to buy?"];
    Choices=choices';
    CurrentChoice=0} 

let createSellEquipmentEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =
    let choices = 
        [{Text="Never Mind!";    Response=Common Cancel}]

    let _, island =  getIsland location playState

    {Location=location;
    Title="Sell Equipment";
    Type=EncounterType.Trade (TradeEncounterType.Equipment Sell);
    Message=["What would you like to sell?"];
    Choices=choices;
    CurrentChoice=0} 

