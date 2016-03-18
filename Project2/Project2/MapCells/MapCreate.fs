module MapCreate

open CellLocation
open MapTerrain
open MapCell
open MapObject
open GameState

let placeIsland (sumLocationsFunc:SumLocationsFunc) (setTerrainFunc:SetTerrainFunc) (location:CellLocation) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    (map, Constants.islandTemplate)
    ||> Map.fold(fun map delta terrain ->
        let mapLocation = sumLocationsFunc location delta
        if map.ContainsKey mapLocation then
            map
            |> setTerrainFunc mapLocation terrain
        else
            map)

let private randomFromList (items:'T list) (random:System.Random) : 'T =
    items
    |> Seq.sortBy (fun e->random.Next())
    |> Seq.head

let private randomConsonant = 
    ["h";"k";"l";"m";"p"]
    |> randomFromList

let private randomVowel = 
    ["a";"e";"i";"o";"u"]
    |> randomFromList

let private generateName (length:int) (consonant:bool) (random:System.Random) : string =
    let generateCharacter (length:int, consonant:bool) : (string * (int * bool)) option= 
        if length = 0 then
            None
        else
            Some ((random |> (if consonant then randomConsonant else randomVowel))    , (length - 1, not consonant ))

    (length,consonant)
    |> Seq.unfold generateCharacter
    |> Seq.reduce (+)

let private generateNames (count:int) (random:System.Random) :string list= 
    let unfolder (count:int, nameSet:Set<string>) : (string * (int * Set<string>)) option=
        if count = nameSet.Count then
            None
        else
            let name = generateName (random.Next(1,4)+random.Next(1,4)+random.Next(1,4)) (random.Next(2)=0) random
            Some (name, (count, nameSet |> Set.add name))

    (count, Set.empty<string>)
    |> Seq.unfold unfolder
    |> List.ofSeq
    |> List.sortBy (fun e->random.Next())

//// Stopped here

let generateIslands (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (distanceFormulaTestFunc:int<cell>->CellLocation->CellLocation->bool) (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) (random:System.Random) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> * CellLocation list =
    let mutable islandLocations = List<CellLocation>.Empty
    let mutable validLocations = Constants.mapLocations |> Array.ofSeq
    while validLocations |> Array.isEmpty |> not do
        let index = random.Next(validLocations |> Array.length)
        let islandLocation = validLocations.[index]
        islandLocations <- islandLocation :: islandLocations
        validLocations <-
            validLocations
            |> Array.filter (distanceFormulaTestFunc Constants.IslandDistance islandLocation)
    //TODO: come up with name list for islands.
    ((map, islandLocations)
    ||> List.fold(fun map location -> 
        map
        |> placeIsland sumLocationsFunc setTerrainFunc location), islandLocations)


let worldObjects =
    [(MapObjectDetail.Boat {Hull=10<health>;MaximumHull=10<health>;GenerateNextStorm=5.0<turn>;Wallet=0.0<currency>;Quest=None},1);
     (MapObjectDetail.Storm {Damage=1<health>},200);
     (MapObjectDetail.Pirate {Hull=5<health>;Attitude=PirateAttitude.Neutral},100);
     (MapObjectDetail.SeaMonster {Health=5<health>;Attitude=SeaMonsterAttitude.Neutral},25);
     (MapObjectDetail.Merfolk {Attitude = MerfolkAttitude.Neutral} ,50)]

let rec generateLocations (worldSize:CellLocation) (random:System.Random) (count:int) (exclusion:Set<CellLocation>) (input:Set<CellLocation>): Set<CellLocation> =
    if input |> Set.count = count then
        input
    else
        let location = {Column = random.Next(worldSize.Column / 1<cell>) * 1<cell>;Row=random.Next(worldSize.Row / 1<cell>) * 1<cell>}
        if not (exclusion.Contains location) then
            input
            |> Set.add location
            |> generateLocations worldSize random count exclusion
        else
            input
            |> generateLocations worldSize random count exclusion

let generateWorldObjects 
    (worldSize:CellLocation) 
    (sumLocationsFunc:CellLocation->CellLocation->CellLocation) 
    (setObjectFunc:CellLocation->MapObject option->CellMap<MapObject>->CellMap<MapObject>) 
    (random:System.Random) 
    (excludedLocations:Set<CellLocation>)
    (actors:Map<CellLocation,MapObject>):Map<CellLocation,MapObject> =
    let allObjects = 
        worldObjects
        |> Seq.map (fun (obj,count)-> [for i = 1 to count do yield obj])
        |> Seq.reduce (@)
        |> Seq.ofList
    generateLocations worldSize random (allObjects |> Seq.length) excludedLocations Set.empty
    |> Set.toSeq
    |> Seq.zip allObjects
    |> Seq.fold (fun actors (detail,loc) -> 
        actors
        |> setObjectFunc loc (Some {CurrentTurn=0.0<turn>;Detail = detail})) actors

let generateIslandObject (name:string) (random:System.Random) :MapObject =
    {CurrentTurn=0.0<turn>;
    Detail=Island {Visits=0;Name=name;Quest={Destination={Column=0<cell>;Row=0<cell>};Reward=0.0<currency>}}}//TODO: generate valid quest
    //TODO: pick an island location


let generateIslandObjects (names:string list) (random:System.Random) (map:CellMap<MapCell>) (originalActors:CellMap<MapObject>) : CellMap<MapObject> =
    ((originalActors,names), map)
    ||> Map.fold(fun (actors,islandNames) location cell -> 
        if cell.Terrain = MapTerrain.Island then
            (actors |> Map.add location (generateIslandObject (islandNames |> List.head) random), islandNames |> List.tail)
        else
            (actors, islandNames))
    |> fst

let createWorld 
    (sumLocationsFunc:CellLocation->CellLocation->CellLocation) 
    (distanceFormulaTestFunc:int<cell>->CellLocation->CellLocation->bool) 
    (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) 
    (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) 
    (setObjectFunc:CellLocation->MapObject option->CellMap<MapObject>->CellMap<MapObject>) 
    (random:System.Random) = 
    let map, islandLocations = 
        Constants.mapLocations
        |> Seq.fold(fun map cellLocation -> 
            map
            |> Map.add cellLocation {Terrain=MapTerrain.DeepWater;Visible=false}
            ) Map.empty<CellLocation,MapCell>
        |> generateIslands sumLocationsFunc distanceFormulaTestFunc setTerrainFunc random
    let actors = 
        generateWorldObjects Constants.WorldSize sumLocationsFunc setObjectFunc random (islandLocations |> Set.ofList) Map.empty
        |> generateIslandObjects (generateNames (islandLocations |> List.length) random) random map
    (actors,map)


