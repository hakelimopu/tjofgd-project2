﻿module BoatMovement

open CellLocation
open MapCell
open MapObject
open GameState
open EncounterHandler
open ActorUpdate
open MapCreate

let generateStorm (generateFlag:bool) (turn:float<turn>) (worldSize:CellLocation) (random:System.Random) (state:PlayState) : PlayState =
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

let moveBoat (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:System.Random) (delta:CellLocation) (state:PlayState) :GameState option =
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

let internal handleKeyDownEventPlayStateFreeMovement (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:System.Random) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    let moveBoatFunc = moveBoat sumLocationsFunc setVisibleFunc worldSize random

    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.F4     -> None

    | SDLKeyboard.ScanCode.KeyPad4
    | SDLKeyboard.ScanCode.Left   -> state |> moveBoatFunc {Column= -1<cell>; Row=  0<cell>}

    | SDLKeyboard.ScanCode.KeyPad6
    | SDLKeyboard.ScanCode.Right  -> state |> moveBoatFunc {Column=  1<cell>; Row=  0<cell>}

    | SDLKeyboard.ScanCode.KeyPad8
    | SDLKeyboard.ScanCode.Up     -> state |> moveBoatFunc {Column=  0<cell>; Row= -1<cell>}

    | SDLKeyboard.ScanCode.KeyPad2
    | SDLKeyboard.ScanCode.Down   -> state |> moveBoatFunc {Column=  0<cell>; Row=  1<cell>}

    | _                           -> state |> PlayState |> Some


