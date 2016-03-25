module IslandUpdate

open GameState
open MapObject
open CellLocation
open Random
open NPCEncounterUtilities

let updateIslandActor 
    (sumLocationsFunc:SumLocationsFunc) 
    (random:RandomFunc) 
    (actorLocation:CellLocation) 
    (actorProperties:IslandProperties) 
    (actorTurn:float<turn>) 
    (currentTurn:float<turn>) 
    (playState:PlayState<_>) :PlayState<_> =
    if currentTurn <= actorTurn then
        playState
    else
        let actors' = 
            playState.Actors 
            |> Map.remove actorLocation

        let actorTurn' = 
            actorTurn + 1.0<turn>

        let actor' = 
            {CurrentTurn = actorTurn';
                Detail      = actorProperties |> Island}

        {playState with Actors = actors' |> Map.add actorLocation actor'}


