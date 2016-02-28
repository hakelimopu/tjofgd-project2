module GameState

open CellLocation
open RenderCell
open MapCell
open MapTerrain
open MapObject

let OutOfBoundsRenderCell = {Character=0xB0uy;Foreground=RenderCellColor.DarkGray;Background=RenderCellColor.Blue}
let UnexploredRenderCell = {Character=0x3Fuy;Foreground=RenderCellColor.Black;Background=RenderCellColor.DarkGray}

let TerrainRenderCells = 
    [(MapTerrain.Water, {Character=0x7Euy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue});
    (MapTerrain.ShallowWater, {Character=0x20uy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue});
    (MapTerrain.Island, {Character=0x1Euy;Foreground=RenderCellColor.Green;Background=RenderCellColor.BrightBlue});
    (MapTerrain.DeepWater, {Character=0xF7uy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue})]
    |> Map.ofSeq

let ObjectRenderCells (detail:MapObject option) =
    match detail with
    | IsBoat        -> {Character=0xF1uy;Foreground=RenderCellColor.Brown;Background=RenderCellColor.BrightBlue}
    | IsStorm       -> {Character=0xF2uy;Foreground=RenderCellColor.BrightYellow;Background=RenderCellColor.BrightBlue}
    | IsPirate      -> {Character=0xF1uy;Foreground=RenderCellColor.Black;Background=RenderCellColor.BrightBlue}
    | IsMerfolk     -> {Character=0x02uy;Foreground=RenderCellColor.Magenta;Background=RenderCellColor.BrightBlue}
    | IsSeaMonster  -> {Character=0xEBuy;Foreground=RenderCellColor.DarkGray;Background=RenderCellColor.BrightBlue}
    | IsIsland      -> {Character=0x1Euy;Foreground=RenderCellColor.Green;Background=RenderCellColor.BrightBlue}
    | IsNothing     -> {Character=0x00uy;Foreground=RenderCellColor.Black;Background=RenderCellColor.Black}

let renderCellForMapCell (actor:MapObject option) (mapCell:MapCell option) :RenderCell =
    match actor, mapCell with
    | Some _, Some x when x.Visible -> ObjectRenderCells actor
    | Some _, Some x when not x.Visible -> UnexploredRenderCell
    | None, Some x when x.Visible -> TerrainRenderCells.[mapCell.Value.Terrain]
    | None, Some x when not x.Visible -> UnexploredRenderCell
    | _, _ -> OutOfBoundsRenderCell

type EncounterType =
    | RanIntoStorm
    | DockedWithIsland

type EncounterReponse =
    | Confirm
    | Cancel
    | Repair

type EncounterChoice =
    {Response:EncounterReponse;
    Text:string}

type EncounterDetail =
    {Location:CellLocation;
    Type:EncounterType;
    Title:string;
    Message:string list;
    Choices:EncounterChoice list;
    CurrentChoice:int}

let getEncounterResponse (detail:EncounterDetail) :EncounterReponse =
    let choiceArray = 
        detail.Choices
        |> Array.ofList
    choiceArray.[detail.CurrentChoice].Response

let getEncounterDetailRows (details:EncounterDetail) : int<cell> =
    1<cell>
    + (if details.Message.Length> 0 then details.Message.Length * 1<cell> + 1<cell> else 0<cell>)
    + (if details.Choices.Length> 0 then details.Choices.Length * 1<cell> + 1<cell> else 0<cell>)

type Encounters =
    | PCEncounter of EncounterDetail
    | NPCEncounters of EncounterDetail list

type PlayState =
    {RenderGrid:CellMap<RenderCell>;
    Encounters:Encounters option;
    Actors:CellMap<MapObject>;
    MapGrid:CellMap<MapCell>}

let (|FreeMovement|HasPCEncounter|HasNPCEncounters|) (playState:PlayState) =
    match playState.Encounters with
    | Some (PCEncounter _) -> HasPCEncounter
    | Some (NPCEncounters _) -> HasNPCEncounters
    | None -> FreeMovement

type GameState = 
    | PlayState of PlayState

let getBoat (state:PlayState) : CellLocation * float<turn> * BoatProperties=
    state.Actors
    |> Map.pick (fun location cell -> 
        match cell.Detail with
        | Boat boatProps -> (location,cell.CurrentTurn,boatProps) |> Some
        | _ -> None)

let getStorm (location:CellLocation) (state:PlayState) : float<turn> * StormProperties =
    let storm = state.Actors.[location]
    (storm.CurrentTurn,
        match storm.Detail with
        | Storm props -> props
        | _ -> raise (new System.NotImplementedException()))

let updateVisibleFlags (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (state:PlayState) :Map<CellLocation,MapCell> =
    let location, _, _ = state |> getBoat
    (state.MapGrid, Constants.visibilityTemplate)
    ||> Seq.fold(fun map delta -> 
        let visibleLocation = delta |> sumLocations location
        map
        |> setVisibleFunc visibleLocation
        )

