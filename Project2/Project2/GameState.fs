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
    | Island = 0
    | ShallowWater = 1
    | Water = 2
    | DeepWater = 3

let TerrainRenderCells = 
    [(MapTerrain.Water, {Character=0x7Euy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue});
    (MapTerrain.ShallowWater, {Character=0x20uy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue});
    (MapTerrain.Island, {Character=0x1Euy;Foreground=RenderCellColor.Green;Background=RenderCellColor.BrightBlue});
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
let IslandDistance = 20<cell>

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

let mapLocations = 
    [0 .. (MapColumns/1<cell>)-1]
    |> Seq.map(fun column-> 
        [0 .. (MapRows / 1<cell>)-1]
        |> Seq.map(fun row-> 
            {Column=column * 1<cell>;Row=row * 1<cell>}))
    |> Seq.reduce (Seq.append)

let islandTemplate = 
    [(-3,-3,MapTerrain.DeepWater);
    (-3,-2,MapTerrain.DeepWater);
    (-3,-1,MapTerrain.Water);
    (-3, 0,MapTerrain.Water);
    (-3, 1,MapTerrain.Water);
    (-3, 2,MapTerrain.DeepWater);
    (-3, 3,MapTerrain.DeepWater);

    (-2,-3,MapTerrain.DeepWater);
    (-2,-2,MapTerrain.Water);
    (-2,-1,MapTerrain.Water);
    (-2, 0,MapTerrain.Water);
    (-2, 1,MapTerrain.Water);
    (-2, 2,MapTerrain.Water);
    (-2, 3,MapTerrain.DeepWater);
    
    (-1,-3,MapTerrain.Water);
    (-1,-2,MapTerrain.Water);
    (-1,-1,MapTerrain.ShallowWater);
    (-1, 0,MapTerrain.ShallowWater);
    (-1, 1,MapTerrain.ShallowWater);
    (-1, 2,MapTerrain.Water);
    (-1, 3,MapTerrain.Water);
    
    ( 0,-3,MapTerrain.Water);
    ( 0,-2,MapTerrain.Water);
    ( 0,-1,MapTerrain.ShallowWater);
    ( 0, 0,MapTerrain.Island);
    ( 0, 1,MapTerrain.ShallowWater);
    ( 0, 2,MapTerrain.Water);
    ( 0, 3,MapTerrain.Water);
    
    ( 3,-3,MapTerrain.DeepWater);
    ( 3,-2,MapTerrain.DeepWater);
    ( 3,-1,MapTerrain.Water);
    ( 3, 0,MapTerrain.Water);
    ( 3, 1,MapTerrain.Water);
    ( 3, 2,MapTerrain.DeepWater);
    ( 3, 3,MapTerrain.DeepWater);

    ( 2,-3,MapTerrain.DeepWater);
    ( 2,-2,MapTerrain.Water);
    ( 2,-1,MapTerrain.Water);
    ( 2, 0,MapTerrain.Water);
    ( 2, 1,MapTerrain.Water);
    ( 2, 2,MapTerrain.Water);
    ( 2, 3,MapTerrain.DeepWater);
      
    ( 1,-3,MapTerrain.Water);
    ( 1,-2,MapTerrain.Water);
    ( 1,-1,MapTerrain.ShallowWater);
    ( 1, 0,MapTerrain.ShallowWater);
    ( 1, 1,MapTerrain.ShallowWater);
    ( 1, 2,MapTerrain.Water);
    ( 1, 3,MapTerrain.Water)]
    |> List.map (fun (x,y,t) -> ({Column=x * 1<cell>;Row=y * 1<cell>},t))
    |> Map.ofList

let placeIsland (location:CellLocation) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    islandTemplate
    |> Map.fold(fun map delta terrain ->
        let mapLocation = {Column=location.Column+delta.Column;Row=location.Row+delta.Row}
        if map.ContainsKey mapLocation then
            map
            |> setTerrain mapLocation terrain
        else
            map) map

let generateIslands (random:System.Random) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    let mutable islandLocations = List<CellLocation>.Empty
    let mutable validLocations = mapLocations |> Array.ofSeq
    while validLocations |> Array.isEmpty |> not do
        let index = random.Next(validLocations |> Array.length)
        let islandLocation = validLocations.[index]
        islandLocations <- islandLocation :: islandLocations
        validLocations <-
            validLocations
            |> Array.filter (fun cellLocation -> 
                let deltaX = cellLocation.Column - islandLocation.Column
                let deltaY = cellLocation.Row - islandLocation.Row
                (deltaX*deltaX+deltaY*deltaY) > IslandDistance * IslandDistance)
    islandLocations
    |> List.fold(fun map location -> 
        map
        |> placeIsland location) map

let createWorld (random:System.Random) = 
    mapLocations
    |> Seq.fold(fun map cellLocation -> 
        map
        |> Map.add cellLocation {Terrain=MapTerrain.DeepWater;Object=None}
        ) Map.empty<CellLocation,MapCell>
    |> generateIslands random
    |> setObject {Column = random.Next(MapColumns / 1<cell>) * 1<cell>;Row=random.Next(MapRows / 1<cell>)*1<cell>} (Some MapObject.Boat)
