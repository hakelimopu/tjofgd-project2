module ActorUpdate

open GameState
open CellLocation
open MapObject
open Random
open StormUpdator

let updateActor (sumLocationsFunc:SumLocationsFunc) (random:RandomFunc) (currentTurn:float<turn>) (playState:PlayState<_>, flag:bool) (actorLocation:CellLocation) (actor:MapObject):PlayState<_> * bool=
    if actor.CurrentTurn >= currentTurn then
        //nothing happens!
        (playState, flag)
    else
        match actor.Detail with
        | Storm stormProperties -> (playState |> updateStormActor sumLocationsFunc random actorLocation stormProperties actor.CurrentTurn currentTurn, true)
        | _ -> (playState, flag)

let rec updateActors (sumLocationsFunc:SumLocationsFunc) (random:RandomFunc) (currentTurn:float<turn>) (playState:PlayState<_>) :PlayState<_>=

    let updatedPlayState, flag = 
        ((playState, false), playState.Actors)
        ||> Map.fold (updateActor sumLocationsFunc random currentTurn)

    if flag then
        updatedPlayState |> updateActors sumLocationsFunc random currentTurn
    else
        updatedPlayState



