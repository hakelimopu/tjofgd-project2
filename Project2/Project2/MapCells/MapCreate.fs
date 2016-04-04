module MapCreate

open CellLocation
open MapTerrain
open MapCell
open MapObject
open GameState
open Random

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

let private randomFromList (items:'T list) (random:RandomFunc) : 'T =
    items
    |> Seq.sortBy (fun e->NextInt |> random |> getInt)
    |> Seq.head

let private randomConsonant = 
    ["h";"k";"l";"m";"p"]
    |> randomFromList

let private randomVowel = 
    ["a";"e";"i";"o";"u"]
    |> randomFromList

let private generateName (length:int) (consonant:bool) (random:RandomFunc) : string =
    let generateCharacter (length:int, consonant:bool) : (string * (int * bool)) option = 
        if length = 0 then
            None
        else
            Some ((random |> (if consonant then randomConsonant else randomVowel)), (length - 1, not consonant ))

    (length, consonant)
    |> Seq.unfold generateCharacter
    |> Seq.reduce (+)

let private generateNames (count:int) (random:RandomFunc) :string list= 
    let emitter (count:int, nameSet:Set<string>) : (Set<string> * (int * Set<string>)) option=
        if count = nameSet.Count then
            None
        else
            let name = generateName (((1,4) |> IntRange |> random |> getInt)+((1,4) |> IntRange |> random |> getInt)+((1,4) |> IntRange |> random |> getInt)) ((2 |> MaxInt |> random |> getInt)=0) random
            if nameSet.Contains name then
                Some (Set.empty<string>, (count, nameSet))
            else
                Some ([name] |> Set.ofList, (count, nameSet |> Set.add name))

    (count, Set.empty<string>)
    |> Seq.unfold emitter
    |> Seq.reduce Set.union
    |> Set.toList
    |> List.sortBy (fun e->NextInt |> random |> getInt)

let randomLocation (worldSize:CellLocation) (random:RandomFunc) :CellLocation =
    {Column = ((worldSize.Column / 1<cell>) |> MaxInt |> random |> getInt) * 1<cell>;
     Row=     ((worldSize.Row  / 1<cell>)   |> MaxInt |> random |> getInt) * 1<cell>}            


let generateIslands (sumLocationsFunc:SumLocationsFunc) (checkLocationsFunc:CheckLocationsFunc) (setTerrainFunc:SetTerrainFunc) (worldSize:CellLocation) (random:RandomFunc) (map:CellMap<MapCell>) :CellMap<MapCell> * CellLocation list =
    let rec islandGenerator (worldSize:CellLocation) (current:Set<CellLocation>) (locations:Set<CellLocation>) : Set<CellLocation> =
        if locations |> Set.isEmpty then
            current
        else
            let location = 
                (worldSize, random)
                ||> randomLocation

            if locations.Contains location then
                ((current   |> Set.add location),
                 (locations |> Set.filter (checkLocationsFunc location)))
                ||> islandGenerator worldSize 
            else
                islandGenerator worldSize current locations

    let start = System.DateTime.Now

    let islandLocations =
        Constants.mapLocations
        |> islandGenerator worldSize Set.empty
        |> Set.toList

    let elapsed = System.DateTime.Now - start
    elapsed.ToString() |> sprintf "Time taken to generate islands: %s" |> SDL.Log.log

    let islandPlacer map location =
        map
        |> placeIsland sumLocationsFunc setTerrainFunc location

    ((map, islandLocations) ||> List.fold islandPlacer, islandLocations)


let worldObjects =
    [(MapObjectDetail.Boat {Hull=10<health>;MaximumHull=10<health>;GenerateNextStorm=5.0<turn>;Wallet=20.0<currency>;Quest=None;EquipmentCapacity=1<slot>;Equipment = Seq.empty},1);
     (MapObjectDetail.Storm {Damage=1<health>},200);
     (MapObjectDetail.Pirate {Hull=5<health>;Attitude=PirateAttitude.Neutral},100);
     (MapObjectDetail.SeaMonster {Health=5<health>;Attitude=SeaMonsterAttitude.Neutral},25);
     (MapObjectDetail.Merfolk {Attitude = MerfolkAttitude.Neutral} ,50)]

let generateLocations 
    (worldSize:CellLocation) 
    (random:RandomFunc) 
    (count:int)
    (exclusion:Set<CellLocation>): Set<CellLocation> =

    //TODO: locationEmitter and islandGenerator are more or less the same!
    let locationEmitter (worldSize:CellLocation) (random:RandomFunc) (count:int, locations:Set<CellLocation>) : (Set<CellLocation> * (int * Set<CellLocation>)) option=
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
    (random:RandomFunc) 
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

let private equipmentPrices =
    [(MapObject.FishingNet,10.0<currency>,20.0<currency>);
    (MapObject.Harpoon,25.0<currency>,50.0<currency>);
    (MapObject.Cannon,600.0<currency>,1000.0<currency>)]


let private generateIslandEquipmentPrices (random:RandomFunc) :Map<EquipmentType,float<currency>> =
    let mapper (equipment:MapObject.EquipmentType,low:float<currency>,high:float<currency>) =
        (equipment,low + (randomFloat random) * (high-low))

    equipmentPrices
    |> Seq.map mapper
    |> Map.ofSeq

let generateIslandObject (name:string) (random:RandomFunc) :MapObject =
    {CurrentTurn=0.0<turn>;
    Detail=
        {Visits=0;
        Name=name;
        EquipmentPrices = generateIslandEquipmentPrices random;
        RepairCost = 1.0<currency/health> * (((randomFloat random) * 2.5) + 0.5);
        RepairCostIncrease = 0.01<currency/health> * (((randomFloat random) * 5.0) + 1.0);
        Quest={Destination={Column=0<cell>;Row=0<cell>};Reward=0.0<currency>}} |> Island}


let generateIslandObjects (names:string list) (random:RandomFunc) (map:CellMap<MapCell>) (originalActors:CellMap<MapObject>) : CellMap<MapObject> =
    let folder (actors,islandNames) location cell =
        if cell.Terrain = MapTerrain.Island then
            (actors |> Map.add location (generateIslandObject (islandNames |> List.head) random), islandNames |> List.tail)
        else
            (actors, islandNames)

    ((originalActors,names), map)
    ||> Map.fold folder
    |> fst

let generateQuest (random:RandomFunc) (location:CellLocation) :QuestDetails =
    {Destination=location;Reward=(NextFloat |> random |> getFloat) * 4.0<currency> + 1.0<currency>}

let generateQuests (random:RandomFunc) (originalActors:CellMap<MapObject>) : CellMap<MapObject> =
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

    let folder (locations:seq<CellLocation>) (random:RandomFunc) (actors:CellMap<MapObject>) (location:CellLocation) (actor:MapObject) :CellMap<MapObject> =
        let chosenLocation =
            locations
            |> Seq.filter (fun e->e <> location)
            |> Seq.sortBy (fun e->NextInt |> random |> getInt)
            |> Seq.head
        let quest = chosenLocation |> generateQuest random
        let modifiedDetail =
            match actor.Detail with
            | Island props -> {props with Quest=quest} |> Island
            | _            -> raise(new System.NotImplementedException())
        actors
        |> Map.add location {actor with Detail = modifiedDetail}

    (originalActors, islands)
    ||> Map.fold (folder islandLocations random)

let createWorld 
    (sumLocationsFunc:SumLocationsFunc) 
    (checkLocationsFunc:CheckLocationsFunc) 
    (setVisibleFunc:SetVisibleFunc) 
    (setTerrainFunc:SetTerrainFunc) 
    (setObjectFunc:SetObjectFunc) 
    (worldSize:CellLocation)
    (random:RandomFunc) = 

    let mapFiller map cellLocation =
        map
        |> Map.add cellLocation {Terrain=MapTerrain.DeepWater;Visible=false}

    let map, islandLocations = 
        (Map.empty<CellLocation,MapCell>, Constants.mapLocations)
        ||> Seq.fold mapFiller 
        |> generateIslands sumLocationsFunc checkLocationsFunc setTerrainFunc worldSize random

    let islandNames = generateNames (islandLocations |> List.length) random

    let actors = 
        generateWorldObjects worldSize sumLocationsFunc setObjectFunc random (islandLocations |> Set.ofList) Map.empty
        |> generateIslandObjects islandNames random map
        |> generateQuests random

    (actors,map)


