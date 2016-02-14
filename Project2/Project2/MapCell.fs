module MapCell

open CellLocation

let MapColumns = 256<cell>
let MapRows = 256<cell>
let IslandDistance = 20<cell>

type MapTerrain =
    | Island = 0
    | ShallowWater = 1
    | Water = 2
    | DeepWater = 3

type MapObject =
    | Boat = 0

type MapCell =
    {Terrain:MapTerrain;
    Object:MapObject option;
    Visible:bool}

let setTerrain (cellLocation:CellLocation) (mapTerrain:MapTerrain) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    let originalCell = 
        cellMap.TryFind(cellLocation)
    let newCell =
        if originalCell.IsSome then
            {originalCell.Value with Terrain=mapTerrain}
        else
            {Terrain=mapTerrain;Object=None;Visible=false}
    cellMap
    |> Map.add cellLocation newCell

let setVisible (cellLocation:CellLocation) (cellMap:CellMap<MapCell>) :CellMap<MapCell> = 
    match cellMap.TryFind(cellLocation) with
    | None -> cellMap
    | Some mapCell -> 
        cellMap
        |> Map.add cellLocation {mapCell with Visible=true}


let setObject (cellLocation:CellLocation) (mapObject:MapObject option) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    let originalCell =
        cellMap.[cellLocation]
    let newCell =
        {originalCell with Object = mapObject}
    cellMap
    |> Map.add cellLocation newCell

let getPlayerLocation (mapGrid:CellMap<MapCell>) = 
    mapGrid
    |> Map.tryPick (fun location cell -> 
        match cell.Object with
        | Some MapObject.Boat -> location |> Some
        | _ -> None)

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

let private visibilityTemplate = 
    [(-2,-1);
    (-2, 0);
    (-2, 1);
    (-1,-2);
    (-1,-1);
    (-1, 0);
    (-1, 1);
    (-1, 2);
    ( 0,-2);
    ( 0,-1);
    ( 0, 0);
    ( 0, 1);
    ( 0, 2);
    ( 1,-2);
    ( 1,-1);
    ( 1, 0);
    ( 1, 1);
    ( 1, 2);
    ( 2,-1);
    ( 2, 0);
    ( 2, 1)]
    |> Seq.map(fun (x,y) -> {Column=x*1<cell>;Row=y*1<cell>})

let updateVisibleFlags (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    let playerLocation = map |> getPlayerLocation
    match playerLocation with
    | None -> map
    | Some location ->
        visibilityTemplate
        |> Seq.fold(fun map delta -> 
            let visibleLocation = delta |> sumLocations location
            map
            |> setVisible visibleLocation
            ) map

let createWorld (random:System.Random) = 
    mapLocations
    |> Seq.fold(fun map cellLocation -> 
        map
        |> Map.add cellLocation {Terrain=MapTerrain.DeepWater;Object=None;Visible=false}
        ) Map.empty<CellLocation,MapCell>
    |> generateIslands random
    |> setObject {Column = random.Next(MapColumns / 1<cell>) * 1<cell>;Row=random.Next(MapRows / 1<cell>)*1<cell>} (Some MapObject.Boat)
    |> updateVisibleFlags




