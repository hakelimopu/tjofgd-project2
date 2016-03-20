module MapCreate

open CellLocation
open MapTerrain
open MapCell
open MapObject
open GameState

let placeIsland (sumLocationsFunc:SumLocationsFunc) (setTerrainFunc:SetTerrainFunc) (location:CellLocation) (map:CellMap<MapCell>) :CellMap<MapCell> =
    let foldFunc (map:CellMap<MapCell>) (delta:CellLocation) (terrain:MapTerrain) :CellMap<MapCell> =
        let mapLocation = (location, delta) ||> sumLocationsFunc

        if map.ContainsKey mapLocation then
            map
            |> setTerrainFunc mapLocation terrain
        else
            map

    (map, Constants.islandTemplate)
    ||> Map.fold foldFunc

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
    let generateCharacter (length:int, consonant:bool) : (string * (int * bool)) option = 
        if length = 0 then
            None
        else
            Some ((random |> (if consonant then randomConsonant else randomVowel)), (length - 1, not consonant ))

    (length, consonant)
    |> Seq.unfold generateCharacter
    |> Seq.reduce (+)

let private generateNames (count:int) (random:System.Random) :string list= 
    let emitter (count:int, nameSet:Set<string>) : (Set<string> * (int * Set<string>)) option=
        if count = nameSet.Count then
            None
        else
            let name = generateName (random.Next(1,4)+random.Next(1,4)+random.Next(1,4)) (random.Next(2)=0) random
            if nameSet.Contains name then
                Some (Set.empty<string>, (count, nameSet))
            else
                Some ([name] |> Set.ofList, (count, nameSet |> Set.add name))

    (count, Set.empty<string>)
    |> Seq.unfold emitter
    |> Seq.reduce Set.union
    |> Set.toList
    |> List.sortBy (fun e->random.Next())

let randomLocation (worldSize:CellLocation) (random:System.Random) :CellLocation =
    {Column = random.Next(worldSize.Column / 1<cell>) * 1<cell>;
     Row=random.Next(worldSize.Row / 1<cell>) * 1<cell>}            


let generateIslands (sumLocationsFunc:SumLocationsFunc) (distanceFormulaTestFunc:DistanceFormulaTestFunc) (setTerrainFunc:SetTerrainFunc) (worldSize:CellLocation) (random:System.Random) (map:CellMap<MapCell>) :CellMap<MapCell> * CellLocation list =
    let islandGenerator (worldSize:CellLocation) (locations:Set<CellLocation>) : (Set<CellLocation> * Set<CellLocation>) option=
        if locations |> Seq.isEmpty then
            None
        else
            let location = 
                (worldSize, random)
                ||> randomLocation

            if locations.Contains location then
                Some ([location] |> Set.ofSeq, locations |> Set.filter (distanceFormulaTestFunc Constants.IslandDistance location))//TODO: get rid of Constants.IslandDistance
            else
                Some (Set.empty<CellLocation>, locations)

    let islandLocations =
        Constants.mapLocations
        |> Seq.unfold (islandGenerator worldSize)
        |> Seq.reduce Set.union
        |> List.ofSeq

    let islandPlacer map location =
        map
        |> placeIsland sumLocationsFunc setTerrainFunc location

    ((map, islandLocations) ||> List.fold islandPlacer, islandLocations)


let worldObjects =
    [(MapObjectDetail.Boat {Hull=10<health>;MaximumHull=10<health>;GenerateNextStorm=5.0<turn>;Wallet=0.0<currency>;Quest=None},1);
     (MapObjectDetail.Storm {Damage=1<health>},200);
     (MapObjectDetail.Pirate {Hull=5<health>;Attitude=PirateAttitude.Neutral},100);
     (MapObjectDetail.SeaMonster {Health=5<health>;Attitude=SeaMonsterAttitude.Neutral},25);
     (MapObjectDetail.Merfolk {Attitude = MerfolkAttitude.Neutral} ,50)]

let generateLocations 
    (worldSize:CellLocation) 
    (random:System.Random) 
    (count:int)
    (exclusion:Set<CellLocation>): Set<CellLocation> =

    //TODO: locationEmitter and islandGenerator are more or less the same!
    let locationEmitter (worldSize:CellLocation) (random:System.Random) (count:int, locations:Set<CellLocation>) : (Set<CellLocation> * (int * Set<CellLocation>)) option=
        if count = locations.Count then        
            None
        else
            let location = 
                (worldSize, random) ||> randomLocation

            if (locations.Contains location) || (exclusion.Contains location) then
                Some (Set.empty<CellLocation>, (count, locations))
            else
                Some ([location] |> Set.ofList, (count - 1, locations.Remove location))

    (count, Set.empty<CellLocation>)
    |> Seq.unfold (locationEmitter worldSize random)
    |> Seq.reduce Set.union

let generateWorldObjects 
    (worldSize:CellLocation) 
    (sumLocationsFunc:SumLocationsFunc) 
    (setObjectFunc:SetObjectFunc) 
    (random:System.Random) 
    (excludedLocations:Set<CellLocation>)
    (actors:Map<CellLocation,MapObject>):Map<CellLocation,MapObject> =

    let duplicator (obj:MapObjectDetail,count:int) = 
        let emitter (obj,count) =
            if count = 0 then
                None
            else
                Some (obj, (obj, count - 1))
                
        (obj, count)
        |> Seq.unfold emitter

    let allObjects = 
        worldObjects
        |> Seq.map duplicator
        |> Seq.reduce (Seq.append)

    let objectPlacer actors (detail,loc) =
        actors
        |> setObjectFunc loc (Some {CurrentTurn=0.0<turn>;Detail = detail})

    (actors,
     generateLocations worldSize random (allObjects |> Seq.length) excludedLocations
     |> Set.toSeq
     |> Seq.zip allObjects)
    ||> Seq.fold objectPlacer

let generateIslandObject (name:string) (random:System.Random) :MapObject =
    {CurrentTurn=0.0<turn>;
    Detail=Island {Visits=0;Name=name;Quest={Destination={Column=0<cell>;Row=0<cell>};Reward=0.0<currency>}}}//TODO: generate valid quest
    //TODO: pick an island location


let generateIslandObjects (names:string list) (random:System.Random) (map:CellMap<MapCell>) (originalActors:CellMap<MapObject>) : CellMap<MapObject> =
    let folder (actors,islandNames) location cell =
        if cell.Terrain = MapTerrain.Island then
            (actors |> Map.add location (generateIslandObject (islandNames |> List.head) random), islandNames |> List.tail)
        else
            (actors, islandNames)

    ((originalActors,names), map)
    ||> Map.fold folder
    |> fst

let generateQuests (random:System.Random) (originalActors:CellMap<MapObject>) : CellMap<MapObject> =
    let islandFinder (location:CellLocation) (actor:MapObject) : bool=
        match Some actor with
        | IsIsland -> true
        | _        -> false

    let islands = 
        originalActors
        |> Map.filter islandFinder

    let islandLocations =
        islands
        |> Map.toSeq
        |> Seq.map fst

    let folder (locations:seq<CellLocation>) (random:System.Random) (actors:CellMap<MapObject>) (location:CellLocation) (actor:MapObject) :CellMap<MapObject> =
        let chosenLocation =
            locations
            |> Seq.filter (fun e->e <> location)
            |> Seq.sortBy (fun e->random.Next())
            |> Seq.head
        let quest = {Destination=chosenLocation;Reward=(random.Next(1,5) |> float) * 1.0<currency>}
        let modifiedDetail =
            match actor.Detail with
            | Island props -> {props with Quest=quest} |> Island
            | _            -> raise(new System.NotImplementedException())
        actors
        |> Map.add location {actor with Detail = modifiedDetail}

    (originalActors, islands)
    ||> Map.fold (folder islandLocations random)

let createWorld 
    (sumLocationsFunc:CellLocation->CellLocation->CellLocation) 
    (distanceFormulaTestFunc:int<cell>->CellLocation->CellLocation->bool) 
    (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) 
    (setTerrainFunc:CellLocation->MapTerrain->CellMap<MapCell>->CellMap<MapCell>) 
    (setObjectFunc:CellLocation->MapObject option->CellMap<MapObject>->CellMap<MapObject>) 
    (worldSize:CellLocation)
    (random:System.Random) = 

    let mapFiller map cellLocation =
        map
        |> Map.add cellLocation {Terrain=MapTerrain.DeepWater;Visible=false}

    let map, islandLocations = 
        (Map.empty<CellLocation,MapCell>, Constants.mapLocations)
        ||> Seq.fold mapFiller 
        |> generateIslands sumLocationsFunc distanceFormulaTestFunc setTerrainFunc worldSize random

    let islandNames = generateNames (islandLocations |> List.length) random

    let actors = 
        generateWorldObjects worldSize sumLocationsFunc setObjectFunc random (islandLocations |> Set.ofList) Map.empty
        |> generateIslandObjects islandNames random map
        |> generateQuests random

    (actors,map)


