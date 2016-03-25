module StormUpdate

open GameState
open MapObject
open CellLocation
open Random
open NPCEncounterUtilities
open ActorUtilities

let private strikeBoat (stormProperties:StormProperties) (actors:CellMap<MapObject>) (actorLocation:CellLocation) (updatedTurn:float<turn>) (playState:PlayState<_>) :PlayState<_> =
    let updatedActors = 
        actors 
        |> Map.add actorLocation {CurrentTurn=updatedTurn;Detail=Storm stormProperties}

    {playState with Actors = updatedActors} 
    |> addNPCStormEncounter actorLocation

let private strikePirate (actors:CellMap<MapObject>) (otherActor:MapObject) (otherActorLocation:CellLocation) (pirateProperties:PirateProperties) (stormProperties:StormProperties) (playState:PlayState<_>) :PlayState<_> =
    let updatedPirateHull = pirateProperties.Hull-stormProperties.Damage

    if updatedPirateHull <= 0<health> then
        let updatedActors = 
            actors 
            |> Map.remove otherActorLocation

        {playState with Actors = updatedActors}
    else
        let updatedPirateProperties = 
            {pirateProperties with Hull = updatedPirateHull}

        let updatedActor = 
            {CurrentTurn = otherActor.CurrentTurn; 
                Detail = Pirate updatedPirateProperties}

        let updatedActors = 
            actors 
            |> Map.add otherActorLocation updatedActor

        {playState with Actors = updatedActors}

let combineStorms (actors:CellMap<MapObject>) (updatedTurn:float<turn>) (otherActor:MapObject) (otherActorLocation:CellLocation) (stormProperties:StormProperties) (otherStormProperties:StormProperties) (playState:PlayState<_>) :PlayState<_> =
    let updatedStorm = 
        {CurrentTurn = (updatedTurn + otherActor.CurrentTurn)/2.0; 
            Detail      = {Damage = stormProperties.Damage+otherStormProperties.Damage} |> Storm}

    let updatedActors = 
        actors 
        |> Map.add otherActorLocation updatedStorm
    {playState with Actors = updatedActors}

let updateStormActor 
    (sumLocationsFunc:SumLocationsFunc) 
    (random:RandomFunc) 
    (actorLocation:CellLocation) 
    (actorProperties:StormProperties) 
    (actorTurn:float<turn>) 
    (currentTurn:float<turn>) 
    (playState:PlayState<_>) :PlayState<_> =
    if currentTurn <= actorTurn then
        playState
    else
        let actors' = 
            playState.Actors 
            |> Map.remove actorLocation

        let actorLocation' = 
            actorLocation 
            |> sumLocationsFunc {Column=((-1,2) |> randomIntRange random ) * 1<cell>;Row=((-1,2) |> randomIntRange random ) * 1<cell>}

        let actorTurn' = 
            actorTurn + 0.5<turn>

        let otherActor = actors'.TryFind actorLocation'
        if otherActor.IsNone then
            let actor' = 
                {CurrentTurn = actorTurn';
                 Detail      = actorProperties |> Storm}

            playState
            |> placeActor actors' actorLocation' actor'
        else
            match otherActor.Value.Detail with
            | Boat properties   -> playState |> strikeBoat actorProperties actors' actorLocation currentTurn
            | Storm properties  -> playState |> combineStorms actors' actorTurn' otherActor.Value actorLocation' actorProperties properties
            | Pirate properties -> playState |> strikePirate  actors' otherActor.Value actorLocation' properties actorProperties
            | _                 -> {playState with Actors = actors'}


