module MapObject

[<Measure>] type turn

type BoatProperties =
    {Hull:int;
    MaximumHull:int;
    GenerateNextStorm:float<turn>}

type StormProperties = 
    {Damage:int}

type PirateAttitude =
    | Friendly
    | Neutral
    | Enemy

type PirateProperties =
    {Hull:int;
    Attitude:PirateAttitude}

type SeaMonsterAttitude =
    | Neutral
    | Enraged

type SeaMonsterProperties =
    {Health:int;
    Attitude:SeaMonsterAttitude}


type MerfolkAttitude =
    | Friendly
    | Neutral
    | Wary
    | Enemy

type MerfolkProperties =
    {Attitude:MerfolkAttitude}

type IslandProperties = 
    {Visits:int}

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



