module MapObject

[<Measure>] type turn

type BoatProperties =
    {Hull:int;
    MaximumHull:int}

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

type MapObjectDetail =
    | Boat of BoatProperties
    | Storm of StormProperties
    | Pirate of PirateProperties
    | SeaMonster of SeaMonsterProperties
    | Merfolk of MerfolkProperties

type MapObject =
    {CurrentTurn:float<turn>;
    Detail:MapObjectDetail}

let (|Boat|Storm|SeaMonster|Pirate|Merfolk|Nothing|) (mapObject:MapObject option) =
    match mapObject with
    | None     -> Nothing
    | Some obj ->
        match obj.Detail with
        | Boat _       -> Boat
        | Pirate _     -> Pirate
        | Storm _      -> Storm
        | SeaMonster _ -> SeaMonster
        | Merfolk _    -> Merfolk



