module IslandEncounterDetails

open CellLocation
open GameState
open EncounterDetailUtilities
open MapObject

let private ``can the ship repair?`` (location:CellLocation) (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    let damage = boatProperties.MaximumHull - boatProperties.Hull
    if damage > 0<health> then
        let _, island = getIsland location playState
        boatProperties.Wallet >= island.RepairCost * 1.0<health>
    else
        false

let private ``can accept quest?`` (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    boatProperties.Quest.IsNone

let private ``can buy or sell equipment?`` (location:CellLocation) (playState:PlayState<_>) :bool =
    BuySellEncounterDetails.``can buy equipment?`` location playState
    || BuySellEncounterDetails.``can sell equipment?`` playState

let private ``is quest complete?`` (location:CellLocation) (playState:PlayState<_>) :bool =
    let boatProperties = playState |> getBoatProperties
    match boatProperties.Quest with
    | Some quest -> quest.Destination = location
    | _          -> false

let createIslandEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =

    let choices = 
        [({Text="Cast Off!";          Response=Common Cancel},           ``always include choice``);
         ({Text="Repair Ship";        Response=Repair},           ``can the ship repair?`` location);
         ({Text="Buy/Sell Equipment"; Response=EncounterReponse.Trade (Equipment BuyOrSell)}, ``can buy or sell equipment?`` location);
         ({Text="Need work!";         Response=EncounterReponse.Quest Query},       ``can accept quest?``);
         ({Text="Delivery!";          Response=EncounterReponse.Quest Complete},    ``is quest complete?`` location)]
        |> List.filter (filterChoice playState)
        |> List.map fst

    let _,island =  getIsland location playState

    {Location=location;
    Title="Island!";
    Type=DockedWithIsland;
    Message=
        [island.Name |> sprintf "You docked at %s!";
        (if island.Visits.IsSome then island.Visits.Value else 0) |> sprintf "Prior visits: %d";
        "What would you like to do?"];
    Choices=choices;
    CurrentChoice=0} 
