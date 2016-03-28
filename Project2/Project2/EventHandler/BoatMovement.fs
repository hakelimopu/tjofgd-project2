module BoatMovement

open CellLocation
open MapCell
open MapObject
open GameState
open EncounterHandler
open ActorUpdate
open MapCreate
open Random

let generateStorm (generateFlag:bool) (turn:float<turn>) (worldSize:CellLocation) (random:RandomFunc) (state:PlayState<_>) : PlayState<_> =
    if not generateFlag then
        state
    else
        let location = 
            (worldSize,random)
            ||> randomLocation

        if state.Actors.ContainsKey location then
            state
        else
            {state with Actors = state.Actors |> Map.add location {CurrentTurn=turn;Detail = Storm {Damage=1<health>}}}

let moveBoat (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (delta:CellLocation) (state:PlayState<_>) :GameState<_> option =
    let playerLocation, boatTurn, boatProperties = state |> getBoat

    let nextLocation = 
        delta 
        |> sumLocationsFunc playerLocation

    if  nextLocation |> state.MapGrid.ContainsKey |> not then
        {state with Encounters = None} 
        |> PlayState 
        |> Some
    else
        if nextLocation |> state.Actors.ContainsKey then
            let encounter = 
                (nextLocation, state)
                ||> startPCEncounter 

            {state with Encounters=encounter} 
            |> PlayState 
            |> Some
        else
            let updatedBoatTurn = 
                boatTurn + 1.0<turn>

            let spawnStorm, updateBoatProperties = 
                if updatedBoatTurn >= boatProperties.GenerateNextStorm then
                    true, {boatProperties with GenerateNextStorm = boatProperties.GenerateNextStorm + 5.0<turn>}
                else
                    false, boatProperties

            let updatedActors = 
                state.Actors
                |> setObject playerLocation None
                |> setObject nextLocation ({CurrentTurn = updatedBoatTurn;Detail = Boat updateBoatProperties} |> Some)

            let stateWithUpdatedActors = 
                {state with 
                    Actors  = updatedActors}

            let updatedMapGrid= 
                stateWithUpdatedActors
                |> updateVisibleFlags sumLocationsFunc setVisibleFunc

            {stateWithUpdatedActors with 
                MapGrid  = updatedMapGrid}
            |> generateStorm spawnStorm updatedBoatTurn worldSize random 
            |> updateActors sumLocationsFunc random updatedBoatTurn
            |> PlayState
            |> Some



