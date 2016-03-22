module ActorUpdate

open GameState
open CellLocation
open MapObject
open EncounterDetails
open Random

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute>]
exception IncompatibleEncounterType

let addNPCEncounter (encounterDetail:EncounterDetail) (playState:PlayState) :PlayState =
    match playState.Encounters with
    | None                            -> {playState with Encounters = Some (NPCEncounters [encounterDetail])}
    | Some (NPCEncounters detailList) -> {playState with Encounters = Some (NPCEncounters (detailList @ [encounterDetail]))}
    | Some (PCEncounter detail)       -> raise IncompatibleEncounterType

let addNPCStormEncounter (actorLocation:CellLocation) (playState:PlayState) :PlayState =
    (actorLocation |> createStorm,
     playState)
    ||> addNPCEncounter

let private strikeBoat (stormProperties:StormProperties) (actors:CellMap<MapObject>) (actorLocation:CellLocation) (updatedTurn:float<turn>) (playState:PlayState) :PlayState =
    let updatedActors = 
        actors 
        |> Map.add actorLocation {CurrentTurn=updatedTurn;Detail=Storm stormProperties}

    {playState with Actors = updatedActors} 
    |> addNPCStormEncounter actorLocation

let private strikePirate (actors:CellMap<MapObject>) (otherActor:MapObject) (otherActorLocation:CellLocation) (pirateProperties:PirateProperties) (stormProperties:StormProperties) (playState:PlayState) :PlayState =
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

let combineStorms (actors:CellMap<MapObject>) (updatedTurn:float<turn>) (otherActor:MapObject) (otherActorLocation:CellLocation) (stormProperties:StormProperties) (otherStormProperties:StormProperties) (playState:PlayState) :PlayState =
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
    (stormProperties:StormProperties) 
    (stormTurn:float<turn>) 
    (currentTurn:float<turn>) 
    (playState:PlayState) :PlayState =
    if currentTurn <= stormTurn then
        playState
    else
        let originalActors = 
            playState.Actors 
            |> Map.remove actorLocation

        let newStormLocation = 
            actorLocation 
            |> sumLocationsFunc {Column=((-1,2) |> IntRange |> random |> getInt) * 1<cell>;Row=((-1,2) |> IntRange |> random |> getInt) * 1<cell>}

        let updateStormTurn = 
            stormTurn + 0.5<turn>

        let otherActor = originalActors.TryFind newStormLocation
        if otherActor.IsNone then
            let updatedStorm = 
                {CurrentTurn = updateStormTurn;
                 Detail      = stormProperties |> Storm}

            let updatedActors =
                originalActors 
                |> Map.add newStormLocation updatedStorm

            {playState with Actors = updatedActors}
        else
            match otherActor.Value.Detail with
            | Boat boatProperties        -> playState |> strikeBoat stormProperties originalActors actorLocation currentTurn
            | Storm otherStormProperties -> playState |> combineStorms originalActors updateStormTurn otherActor.Value newStormLocation stormProperties otherStormProperties
            | Pirate pirateProperties    -> playState |> strikePirate  originalActors otherActor.Value newStormLocation pirateProperties stormProperties
            | _                          -> {playState with Actors = originalActors}

let updateActor (sumLocationsFunc:SumLocationsFunc) (random:RandomFunc) (actorLocation:CellLocation) (actor:MapObject) (currentTurn:float<turn>) (playState:PlayState, flag:bool) :PlayState * bool=
    if actor.CurrentTurn >= currentTurn then
        //nothing happens!
        (playState, flag)
    else
        match actor.Detail with
        | Storm stormProperties -> (playState |> updateStormActor sumLocationsFunc random actorLocation stormProperties actor.CurrentTurn currentTurn, true)
        | _ -> (playState, flag)

let rec updateActors (sumLocationsFunc:SumLocationsFunc) (random:RandomFunc) (currentTurn:float<turn>) (playState:PlayState) :PlayState=
    let actorUpdater (currentState:PlayState * bool) (location:CellLocation) (actor:MapObject) :PlayState * bool= 
        updateActor sumLocationsFunc random location actor currentTurn currentState

    let updatedPlayState, flag = 
        ((playState, false), playState.Actors)
        ||> Map.fold actorUpdater

    if flag then
        updatedPlayState |> updateActors sumLocationsFunc random currentTurn
    else
        updatedPlayState



