﻿module KeyDownEvent

open CellLocation
open GameState
open EncounterChoice
open BoatMovement
open MapCell

let private handleKeyDownEventDeadState (createFunc:unit->GameState) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.F2 -> createFunc() |> Some
    | _ -> state |> Some

let private handleKeyDownEventPlayState (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:System.Random) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    (keyboardEvent, state)
    ||> match state with
        | FreeMovement     -> handleKeyDownEventPlayStateFreeMovement  sumLocationsFunc setVisibleFunc worldSize random
        | HasPCEncounter   -> handleKeyDownEventPlayStatePCEncounter   sumLocationsFunc setVisibleFunc random
        | HasNPCEncounters -> handleKeyDownEventPlayStateNPCEncounters sumLocationsFunc setVisibleFunc random 

let internal handleKeyDownEvent (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (createFunc:unit->GameState) (worldSize:CellLocation) (random:System.Random) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState) :GameState option =
    match state with
    | PlayState x -> 
        x |> handleKeyDownEventPlayState sumLocationsFunc setVisibleFunc worldSize random keyboardEvent
    | _ -> 
        state |> handleKeyDownEventDeadState createFunc keyboardEvent



