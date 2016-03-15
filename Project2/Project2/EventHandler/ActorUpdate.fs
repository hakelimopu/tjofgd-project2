module ActorUpdate

open GameState
open CellLocation
open MapObject

let addNPCStormEncounter (actorLocation:CellLocation) (playState:PlayState) :PlayState =
    let encounterDetail =
        {Location=actorLocation;
        Title="Storm!";
        Type=RanIntoStorm;
        Message=["You have run into a storm;";"it has damaged your boat!"];
        Choices=[{Text="OK";Response=Confirm}];
        CurrentChoice=0} 
    match playState.Encounters with
    | None -> {playState with Encounters = Some (NPCEncounters [encounterDetail])}
    | Some (NPCEncounters detailList) -> {playState with Encounters = Some (NPCEncounters (detailList |> List.append [encounterDetail]))}
    | Some (PCEncounter detail) -> raise (new System.InvalidOperationException("Cannot add NPC encounter to a PC encounter!"))

let updateStormActor (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (random:System.Random) (actorLocation:CellLocation) (stormProperties:StormProperties) (stormTurn:float<turn>) (currentTurn:float<turn>) (playState:PlayState) :PlayState =
    if currentTurn > stormTurn then
        let originalActors = playState.Actors |> Map.remove actorLocation
        let newStormLocation = actorLocation |> sumLocationsFunc {Column=random.Next(-1,2) * 1<cell>;Row=random.Next(-1,2) * 1<cell>}
        let updateStormTurn = stormTurn + 0.5<turn>
        if originalActors.ContainsKey newStormLocation then
            let otherActor = originalActors.[newStormLocation]
            match otherActor.Detail with
            | Boat boatProperties -> 
                {playState with Actors = (originalActors |> Map.add actorLocation {CurrentTurn=updateStormTurn;Detail=Storm stormProperties})} |> addNPCStormEncounter actorLocation

            | Storm otherStormProperties ->  
                {playState with Actors = (originalActors |> Map.add newStormLocation {CurrentTurn = (updateStormTurn + otherActor.CurrentTurn)/2.0 ; Detail = Storm {Damage=stormProperties.Damage+otherStormProperties.Damage}})}

            | Pirate pirateProperties ->
                let newPirateProperties = {pirateProperties with Hull=pirateProperties.Hull-stormProperties.Damage}
                if newPirateProperties.Hull < 0<health> then
                    {playState with Actors = (originalActors |> Map.remove newStormLocation)}
                else
                    {playState with Actors = (originalActors |> Map.add newStormLocation {CurrentTurn = otherActor.CurrentTurn; Detail = Pirate newPirateProperties})}

            | _ -> 
                {playState with Actors = originalActors}

        else
            {playState with Actors = (originalActors |> Map.add newStormLocation {CurrentTurn=updateStormTurn;Detail=Storm stormProperties} )}
    else
        playState

let rec updateActor (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (random:System.Random) (actorLocation:CellLocation) (actor:MapObject) (currentTurn:float<turn>) (playState:PlayState) :PlayState=
    if actor.CurrentTurn < currentTurn then
        //actor gets a turn!
        //TODO: does the actor still exist on the map at the original location?
        match actor.Detail with
        | Storm stormProperties -> playState |> updateStormActor sumLocationsFunc random actorLocation stormProperties actor.CurrentTurn currentTurn
        | _ -> playState
    else
        //nothing happens!
        playState

let updateActors (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (random:System.Random) (currentTurn:float<turn>) (playState:PlayState) :PlayState=
    (playState, playState.Actors)
    ||> Map.fold (fun currentState location actor -> 
        updateActor sumLocationsFunc random location actor currentTurn currentState)



