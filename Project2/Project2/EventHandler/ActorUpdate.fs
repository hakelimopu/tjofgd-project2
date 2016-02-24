module ActorUpdate

open GameState
open CellLocation
open MapObject

let rec updateActor (actorLocation:CellLocation) (actor:MapObject) (currentTurn:float<turn>) (playState:PlayState) :PlayState=
    if actor.CurrentTurn < currentTurn then
        //actor gets a turn!
        //TODO: does the actor still exist on the map at the original location?
        playState
    else
        //nothing happens!
        playState

let updateActors (currentTurn:float<turn>) (playState:PlayState) :PlayState=
    (playState, playState.Actors)
    ||> Map.fold (fun currentState location actor -> 
        updateActor location actor currentTurn currentState)



