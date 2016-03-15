﻿module MapObject

open CellLocation

[<Measure>] type turn
[<Measure>] type health
[<Measure>] type currency

type QuestDetails =
    {Destination:CellLocation;
    Reward:float<currency>}

//TODO: add cargo capacity, equipment capacity, quests
type BoatProperties =
    {Hull:int<health>;
    MaximumHull:int<health>;
    Wallet:float<currency>;
    Quest:QuestDetails option;
    GenerateNextStorm:float<turn>}

type StormProperties = 
    {Damage:int<health>}

type PirateAttitude =
    | Friendly
    | Neutral
    | Enemy

type PirateProperties =
    {Hull:int<health>;
    Attitude:PirateAttitude}

type SeaMonsterAttitude =
    | Neutral
    | Enraged

type SeaMonsterProperties =
    {Health:int<health>;
    Attitude:SeaMonsterAttitude}


type MerfolkAttitude =
    | Friendly
    | Neutral
    | Wary
    | Enemy

type MerfolkProperties =
    {Attitude:MerfolkAttitude}

//give a name!
type IslandProperties = 
    {Name:string;
    Visits:int;
    Quest:QuestDetails}

type MapObjectDetail =
    | Boat of BoatProperties
    | Storm of StormProperties
    | Pirate of PirateProperties
    | SeaMonster of SeaMonsterProperties
    | Merfolk of MerfolkProperties
    | Island of IslandProperties

type MapObject =
    {CurrentTurn:float<turn>;
    Detail:MapObjectDetail}

let (|IsBoat|IsStorm|IsSeaMonster|IsPirate|IsMerfolk|IsNothing|IsIsland|) (mapObject:MapObject option) =
    match mapObject with
    | None     -> IsNothing
    | Some obj ->
        match obj.Detail with
        | Boat _       -> IsBoat
        | Pirate _     -> IsPirate
        | Storm _      -> IsStorm
        | SeaMonster _ -> IsSeaMonster
        | Merfolk _    -> IsMerfolk
        | Island _     -> IsIsland



