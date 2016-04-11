module MapObject

open CellLocation

[<Measure>] type turn
[<Measure>] type health
[<Measure>] type currency
[<Measure>] type slot

type QuestDetails =
    {Destination:CellLocation;
    Reward:float<currency>}

type EquipmentType =
    | FishingNet
    | Harpoon
    | Cannon

type EquipmentDetails =
    {Type:EquipmentType;
    Size:int<slot>;
    Durability:float<health>;
    MaximumDurability:float<health>}

let equipmentTemplates =
    [(FishingNet,("Fishing Net",{Type=FishingNet;Size=1<slot>;Durability=10.0<health>;MaximumDurability=10.0<health>}));
    (Harpoon,("Harpoon",{Type=Harpoon;Size=1<slot>;Durability=50.0<health>;MaximumDurability=50.0<health>}));
    (Cannon,("Cannon",{Type=Cannon;Size=1<slot>;Durability=100.0<health>;MaximumDurability=100.0<health>}))]
    |> Map.ofSeq

type BoatProperties =
    {Hull:int<health>;
     MaximumHull:int<health>;
     EquipmentCapacity:int<slot>;
     Equipment:seq<EquipmentDetails>;
     //cargo capacity
     Wallet:float<currency>;
     Quest:QuestDetails option;
     BoundFor:CellLocation option;
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
     Visits:int option;
     Quest:QuestDetails;
     EquipmentPrices:Map<EquipmentType,float<currency>>;
     RepairCost:float<currency/health>;
     RepairCostIncrease:float<currency/health>}

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



