module MapCell

open CellLocation

let MapColumns = 256<cell>
let MapRows = 256<cell>
let WorldSize = {Column=MapColumns;Row=MapRows}
let IslandDistance = 20<cell>

type MapTerrain =
    | Island = 0
    | ShallowWater = 1
    | Water = 2
    | DeepWater = 3

type MapObject =
    | Boat
    | Storm
    | Pirate
    | SeaMonster
    | Merfolk

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

let setTerrainWrapped (worldSize:CellLocation) (cellLocation:CellLocation) (mapTerrain:MapTerrain) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    setTerrain (cellLocation |> wrapLocation worldSize) mapTerrain cellMap

let setVisible (cellLocation:CellLocation) (cellMap:CellMap<MapCell>) :CellMap<MapCell> = 
    match cellMap.TryFind(cellLocation) with
    | None -> cellMap
    | Some mapCell -> 
        cellMap
        |> Map.add cellLocation {mapCell with Visible=true}

let setVisibleWrapped (worldSize:CellLocation) (cellLocation:CellLocation) (cellMap:CellMap<MapCell>) :CellMap<MapCell> = 
    setVisible (cellLocation |> wrapLocation worldSize) cellMap
    
let setObject (cellLocation:CellLocation) (mapObject:MapObject option) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    let originalCell =
        cellMap.[cellLocation]
    let newCell =
        {originalCell with Object = mapObject}
    cellMap
    |> Map.add cellLocation newCell

let setObjectWrapped (worldSize:CellLocation) (cellLocation:CellLocation) (mapObject:MapObject option) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    setObject (cellLocation |> wrapLocation worldSize) mapObject cellMap

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

let placeIsland (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) (location:CellLocation) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    islandTemplate
    |> Map.fold(fun map delta terrain ->
        let mapLocation = sumLocationsFunc location delta
        if map.ContainsKey mapLocation then
            map
            |> setTerrainFunc mapLocation terrain
        else
            map) map

let distanceFormulaTest (maximum:int<cell>) (first:CellLocation) (second:CellLocation) :bool =
    let deltaX = second.Column - first.Column
    let deltaY = second.Row - first.Row
    (deltaX*deltaX+deltaY*deltaY) > maximum * maximum

let distanceFormulaTestWrapped (worldSize:CellLocation) (maximum:int<cell>) (first:CellLocation) (second:CellLocation) :bool =
    let locations = 
        [second;
        {second with Column=second.Column+worldSize.Column};
        {second with Row=second.Row+worldSize.Row};
        {Column=second.Column+worldSize.Column;Row=second.Row+worldSize.Row}]
    locations
    |> Seq.fold (fun failures location->
        if distanceFormulaTest maximum first second then
            failures + 1
        else
            failures) 0
    |> (=) locations.Length


let generateIslands (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (distanceFormulaTestFunc:int<cell>->CellLocation->CellLocation->bool) (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) (random:System.Random) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    let mutable islandLocations = List<CellLocation>.Empty
    let mutable validLocations = mapLocations |> Array.ofSeq
    while validLocations |> Array.isEmpty |> not do
        let index = random.Next(validLocations |> Array.length)
        let islandLocation = validLocations.[index]
        islandLocations <- islandLocation :: islandLocations
        validLocations <-
            validLocations
            |> Array.filter (distanceFormulaTestFunc IslandDistance islandLocation)
    islandLocations
    |> List.fold(fun map location -> 
        map
        |> placeIsland sumLocationsFunc setTerrainFunc location) map

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

let updateVisibleFlags (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    let playerLocation = map |> getPlayerLocation
    match playerLocation with
    | None -> map
    | Some location ->
        visibilityTemplate
        |> Seq.fold(fun map delta -> 
            let visibleLocation = delta |> sumLocations location
            map
            |> setVisibleFunc visibleLocation
            ) map

let worldObjects =
    [(MapObject.Boat,1);
    (MapObject.Storm,200);
    (MapObject.Pirate,100);
    (MapObject.SeaMonster,25);
    (MapObject.Merfolk,50)]

let rec generateLocations (worldSize:CellLocation) (random:System.Random) (count:int) (input:Set<CellLocation>): Set<CellLocation> =
    if input |> Set.count = count then
        input
    else
        input
        |> Set.add {Column = random.Next(worldSize.Column / 1<cell>) * 1<cell>;Row=random.Next(worldSize.Row / 1<cell>) * 1<cell>}
        |> generateLocations worldSize random count

let generateWorldObjects 
    (worldSize:CellLocation) 
    (sumLocationsFunc:CellLocation->CellLocation->CellLocation) 
    (setObjectFunc:CellLocation->MapObject option->CellMap<MapCell>->CellMap<MapCell>) 
    (random:System.Random) 
    (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    let allObjects = 
        worldObjects
        |> Seq.map (fun (obj,count)-> [for i = 1 to count do yield obj])
        |> Seq.reduce (@)
        |> Seq.ofList
    generateLocations worldSize random (allObjects |> Seq.length) Set.empty
    |> Set.toSeq
    |> Seq.zip allObjects
    |> Seq.fold (fun map (obj,loc) -> 
        map
        |> setObjectFunc loc (Some obj)) map

let createWorld 
    (sumLocationsFunc:CellLocation->CellLocation->CellLocation) 
    (distanceFormulaTestFunc:int<cell>->CellLocation->CellLocation->bool) 
    (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) 
    (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) 
    (setObjectFunc:CellLocation->MapObject option->CellMap<MapCell>->CellMap<MapCell>) 
    (random:System.Random) = 
    mapLocations
    |> Seq.fold(fun map cellLocation -> 
        map
        |> Map.add cellLocation {Terrain=MapTerrain.DeepWater;Object=None;Visible=false}
        ) Map.empty<CellLocation,MapCell>
    |> generateIslands sumLocationsFunc distanceFormulaTestFunc setTerrainFunc random
    |> generateWorldObjects WorldSize sumLocationsFunc setObjectFunc random
    |> updateVisibleFlags setVisibleFunc




