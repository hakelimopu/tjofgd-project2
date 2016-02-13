module GameState

open SDLUtility

[<Measure>] type cell

type CellLocation =
    {Column:int<cell>;Row:int<cell>}

let pixelsPerColumn = 8<px/cell>
let pixelsPerRow = 8<px/cell>

type RenderCellColor = 
    | Black = 0
    | Blue = 1
    | Green = 2
    | Cyan = 3
    | Red = 4
    | Magenta = 5
    | Brown = 6
    | White = 7
    | DarkGray = 8
    | BrightBlue = 9
    | BrightGreen = 10
    | BrightCyan = 11
    | BrightRed = 12
    | BrightMagenta = 13
    | BrightYellow = 14
    | BrightWhite = 15

type RenderCell =
    {Character:byte;Foreground:RenderCellColor;Background:RenderCellColor}

type CellMap<'T> = Map<CellLocation,'T>

type MapTerrain =
    | Water = 0
    | DeepWater = 1

let TerrainRenderCells = 
    [(MapTerrain.Water, {Character=0x7Euy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue});
    (MapTerrain.DeepWater, {Character=0xF7uy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue})]
    |> Map.ofSeq

type MapObject =
    | Boat = 0

let ObjectRenderCells = 
    [(MapObject.Boat, {Character=0xF1uy;Foreground=RenderCellColor.Brown;Background=RenderCellColor.BrightBlue})]
    |> Map.ofSeq

type MapCell =
    {Terrain:MapTerrain;
    Object:MapObject option}

let renderCellForMapCell (mapCell:MapCell option) :RenderCell =
    if mapCell.IsSome then
        match mapCell.Value.Object with
        | Some x -> ObjectRenderCells.[x]
        | None -> TerrainRenderCells.[mapCell.Value.Terrain]
    else
        TerrainRenderCells.[MapTerrain.DeepWater]

let setTerrain (cellLocation:CellLocation) (mapTerrain:MapTerrain) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    let originalCell = 
        cellMap.TryFind(cellLocation)
    let newCell =
        if originalCell.IsSome then
            {originalCell.Value with Terrain=mapTerrain}
        else
            {Terrain=mapTerrain;Object=None}
    cellMap
    |> Map.add cellLocation newCell

let setObject (cellLocation:CellLocation) (mapObject:MapObject option) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    let originalCell =
        cellMap.[cellLocation]
    let newCell =
        {originalCell with Object = mapObject}
    cellMap
    |> Map.add cellLocation newCell


let MapColumns = 256<cell>
let MapRows = 256<cell>

let getPlayerLocation (mapGrid:CellMap<MapCell>) = 
    mapGrid
    |> Map.tryPick (fun location cell -> 
        match cell.Object with
        | Some MapObject.Boat -> location |> Some
        | _ -> None)


type PlayState =
    {RenderGrid:CellMap<RenderCell>;
    MapGrid:CellMap<MapCell>}

type GameState = 
    | PlayState of PlayState



