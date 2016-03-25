module ActorUtilities

open GameState
open CellLocation
open MapObject

let placeActor (actors:CellMap<MapObject>) (actorLocation:CellLocation) (actor:MapObject) (playState:PlayState<_>) :PlayState<_>=
    let actors' =
        actors 
        |> Map.add actorLocation actor

    {playState with Actors = actors'}
