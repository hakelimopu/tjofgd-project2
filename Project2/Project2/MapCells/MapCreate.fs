module MapCreate

open CellLocation
open MapTerrain
open MapCell
open MapObject

let placeIsland (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) (location:CellLocation) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    Constants.islandTemplate
    |> Map.fold(fun map delta terrain ->
        let mapLocation = sumLocationsFunc location delta
        if map.ContainsKey mapLocation then
            map
            |> setTerrainFunc mapLocation terrain
        else
            map) map


let generateIslands (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (distanceFormulaTestFunc:int<cell>->CellLocation->CellLocation->bool) (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) (random:System.Random) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    let mutable islandLocations = List<CellLocation>.Empty
    let mutable validLocations = Constants.mapLocations |> Array.ofSeq
    while validLocations |> Array.isEmpty |> not do
        let index = random.Next(validLocations |> Array.length)
        let islandLocation = validLocations.[index]
        islandLocations <- islandLocation :: islandLocations
        validLocations <-
            validLocations
            |> Array.filter (distanceFormulaTestFunc Constants.IslandDistance islandLocation)
    islandLocations
    |> List.fold(fun map location -> 
        map
        |> placeIsland sumLocationsFunc setTerrainFunc location) map


let worldObjects =
    [(MapObjectDetail.Boat {Hull=10;MaximumHull=10},1);
    (MapObjectDetail.Storm {Damage=1},200);
    (MapObjectDetail.Pirate {Hull=5;Attitude=PirateAttitude.Neutral},100);
    (MapObjectDetail.SeaMonster {Health=5;Attitude=SeaMonsterAttitude.Neutral},25);
    (MapObjectDetail.Merfolk {Attitude = MerfolkAttitude.Neutral} ,50)]

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
    (setObjectFunc:CellLocation->MapObject option->CellMap<MapObject>->CellMap<MapObject>) 
    (random:System.Random) 
    (actors:Map<CellLocation,MapObject>):Map<CellLocation,MapObject> =
    let allObjects = 
        worldObjects
        |> Seq.map (fun (obj,count)-> [for i = 1 to count do yield obj])
        |> Seq.reduce (@)
        |> Seq.ofList
    generateLocations worldSize random (allObjects |> Seq.length) Set.empty
    |> Set.toSeq
    |> Seq.zip allObjects
    |> Seq.fold (fun actors (detail,loc) -> 
        actors
        |> setObjectFunc loc (Some {CurrentTurn=0.0<turn>;Detail = detail})) actors

let createWorld 
    (sumLocationsFunc:CellLocation->CellLocation->CellLocation) 
    (distanceFormulaTestFunc:int<cell>->CellLocation->CellLocation->bool) 
    (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) 
    (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) 
    (setObjectFunc:CellLocation->MapObject option->CellMap<MapObject>->CellMap<MapObject>) 
    (random:System.Random) = 
    let map = 
        Constants.mapLocations
        |> Seq.fold(fun map cellLocation -> 
            map
            |> Map.add cellLocation {Terrain=MapTerrain.DeepWater;Visible=false}
            ) Map.empty<CellLocation,MapCell>
        |> generateIslands sumLocationsFunc distanceFormulaTestFunc setTerrainFunc random
    let actors = 
        generateWorldObjects Constants.WorldSize sumLocationsFunc setObjectFunc random Map.empty
    (actors, (actors,map)
    ||> updateVisibleFlags setVisibleFunc)


