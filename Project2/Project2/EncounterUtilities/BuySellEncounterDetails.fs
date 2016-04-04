module BuySellEncounterDetails

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

let createBuySellEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =
    let choices = 
        [({Text="Never Mind!";    Response=Cancel},        ``always include choice``);
         ({Text="Buy Equipment";  Response=BuyEquipment},  ``can buy equipment?`` location);
         ({Text="Sell Equipment"; Response=SellEquipment}, ``can sell equipment?``)]
        |> List.filter (filterChoice playState)
        |> List.map fst

    let _, island =  getIsland location playState

    let quest = island.Quest

    let _, island' = getIsland quest.Destination playState

    {Location=location;
    Title="Buy/Sell Equipment";
    Type=EncounterType.BuySellEquipment;
    Message=["What would you like to do?"];
    Choices=choices;
    CurrentChoice=0} 

