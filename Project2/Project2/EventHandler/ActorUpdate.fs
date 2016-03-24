module ActorUpdate

open GameState
open CellLocation
open MapObject
open Random
open StormUpdater
open MerfolkUpdate
open SeaMonsterUpdate
open PirateUpdater

let updateActor (sumLocationsFunc:SumLocationsFunc) (random:RandomFunc) (currentTurn:float<turn>) (playState:PlayState<_>, flag:bool) (actorLocation:CellLocation) (actor:MapObject):PlayState<_> * bool=
    if actor.CurrentTurn >= currentTurn then
        //nothing happens!
        (playState, flag)
    else
        match actor.Detail with
        | Storm properties      -> playState |> updateStormActor sumLocationsFunc random actorLocation properties actor.CurrentTurn currentTurn, true
        | Pirate properties     -> playState |> updatePirateActor sumLocationsFunc random actorLocation properties actor.CurrentTurn currentTurn, flag
        | SeaMonster properties -> playState |> updateSeaMonsterActor sumLocationsFunc random actorLocation properties actor.CurrentTurn currentTurn, flag
        | Merfolk properties    -> playState |> updateMerfolkActor sumLocationsFunc random actorLocation properties actor.CurrentTurn currentTurn, flag
        | _                     -> playState, flag

let rec updateActors (sumLocationsFunc:SumLocationsFunc) (random:RandomFunc) (currentTurn:float<turn>) (playState:PlayState<_>) :PlayState<_>=

    let playState', flag = 
        ((playState, false), playState.Actors)
        ||> Map.fold (updateActor sumLocationsFunc random currentTurn)

    if flag then
        playState' |> updateActors sumLocationsFunc random currentTurn
    else
        playState'



