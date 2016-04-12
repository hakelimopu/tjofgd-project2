module GameState

open CellLocation
open RenderCell
open MapCell
open MapTerrain
open MapObject


type QueryEncounterType =
    | Quest
    | Repair

type TradeEncounterSubtype =
    | BuyOrSell
    | Buy
    | Sell

type TradeEncounterType =
    | Equipment of TradeEncounterSubtype

type MenuType =
    | Main
    | Game
    | Boat
    | Island
    | Options

type EncounterType =
    | RanIntoStorm
    | DockedWithIsland
    | Query of QueryEncounterType
    | Trade of TradeEncounterType
    | Menu of MenuType

type CommonEncounterResponse =
    | Confirm //also used for yes, ok
    | Cancel  //also used for no
    | Nada

type QuestEncounterResponse =
    | Query
    | Complete

type PurchaseEncounterResponse =
    | Equipment of EquipmentType

type SaleEncounterResponse =
    | Equipment of int

type GameCommandType =
    | New
    | Load
    | Save
    | Quit

type EncounterResponse =
    | Common of CommonEncounterResponse
    | Repair
    | Quest of QuestEncounterResponse
    | Trade of TradeEncounterType
    | Purchase of PurchaseEncounterResponse
    | Sale of SaleEncounterResponse
    | Menu of MenuType
    | GameCommand of GameCommandType

type EncounterChoice =
    {Response:EncounterResponse;
     Text:string}

type EncounterDetail =
    {Location:CellLocation;
     Type:EncounterType;
     Title:string;
     Message:string list;
     Choices:EncounterChoice list;
     CurrentChoice:int;
     WindowSize:int;
     WindowIndex:int}


type MoveCommand =
    | North
    | East
    | South
    | West

type MenuCommand =
    | Next
    | Previous
    | Select

type CommandType =
    | Quit
    | Move of MoveCommand
    | Menu of MenuCommand

let getEncounterResponse (detail:EncounterDetail) :EncounterResponse =
    let choiceArray = 
        detail.Choices
        |> Array.ofList
    choiceArray.[detail.CurrentChoice].Response

let getEncounterDetailRows (details:EncounterDetail) : int<cell> =
    let zeroOrAddOne n = 
        if n> 0 then 
            n * 1<cell> + 1<cell> 
        else 
            0<cell>

    1<cell>
    + (details.Message.Length |> zeroOrAddOne)
    + (details.Choices.Length |> zeroOrAddOne)

type Encounters =
    | PCEncounter   of EncounterDetail
    | NPCEncounters of EncounterDetail list

type PlayState<'TRender> =
    {RenderData:'TRender;
     Encounters:Encounters option;
     Actors:CellMap<MapObject>;
     MapGrid:CellMap<MapCell>}

let (|FreeMovement|HasEncounter|) (playState:PlayState<_>) =
    match playState.Encounters with
    | Some (PCEncounter _)   -> HasEncounter
    | Some (NPCEncounters _) -> HasEncounter
    | None                   -> FreeMovement

type GameState<'TRender> = 
    | PlayState of PlayState<'TRender>
    | DeadState of PlayState<'TRender>

let getBoat (state:PlayState<_>) : CellLocation * float<turn> * BoatProperties=
    let picker location cell = 
        match cell.Detail with
        | MapObject.Boat boatProps -> (location,cell.CurrentTurn,boatProps) |> Some
        | _              -> None

    state.Actors
    |> Map.pick picker

let getBoatProperties (state:PlayState<_>) : BoatProperties =
    let _,_,props = state |> getBoat
    props

let setBoatProperties (properties:BoatProperties) (state:PlayState<_>) : PlayState<_> =
    let location, turn, _ = state |> getBoat
    {state with Actors = state.Actors |> Map.add location {CurrentTurn = turn; Detail = properties |> MapObject.Boat}}

let getBoatLocation (state:PlayState<_>) : CellLocation =
    let location,_,_ = state |> getBoat
    location

let getCurrency (state:PlayState<_>) : float<currency> =
    (state |> getBoatProperties).Wallet

let setCurrency (amount:float<currency>) (state:PlayState<_>) :PlayState<_> =
    let where,turn,props = state |> getBoat
    {state with Actors = state.Actors |> Map.add where {CurrentTurn=turn;Detail={props with Wallet=amount} |> MapObject.Boat}}

let getStorm (location:CellLocation) (state:PlayState<_>) : float<turn> * StormProperties =
    let storm = state.Actors.[location]
    (storm.CurrentTurn,
        match storm.Detail with
        | Storm props -> props
        | _ -> raise (new System.NotImplementedException()))

//TODO: this is more or less the same function as getStorm!
let getIsland (location:CellLocation) (state:PlayState<_>) : float<turn> * IslandProperties =
    let island = state.Actors.[location]
    (island.CurrentTurn,
        match island.Detail with
        | MapObject.Island props -> props
        | _ -> raise (new System.NotImplementedException()))

let updateVisibleFlags (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (state:PlayState<_>) :Map<CellLocation,MapCell> =
    let updateVisibility sumLocationsFunc setVisibleFunc location map delta = 
        map
        |> setVisibleFunc ((location, delta) ||> sumLocationsFunc)

    (state.MapGrid, Constants.visibilityTemplate)
    ||> Seq.fold (updateVisibility sumLocationsFunc setVisibleFunc (state |> getBoatLocation))

