module KeyDownEvent

open CellLocation
open GameState
open EncounterChoice
open BoatMovement
open MapCell
open Random

let private handleKeyDownEventDeadState (createFunc:unit->GameState<_>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState<_>) :GameState<_> option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.F2 -> createFunc() |> Some
    | _ -> state |> Some

let private handleKeyDownEventPlayState (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    (keyboardEvent, state)
    ||> match state with
        | FreeMovement     -> handleKeyDownEventPlayStateFreeMovement  sumLocationsFunc setVisibleFunc worldSize random
        | HasPCEncounter   -> handleKeyDownEventPlayStatePCEncounter   sumLocationsFunc setVisibleFunc random
        | HasNPCEncounters -> handleKeyDownEventPlayStateNPCEncounters sumLocationsFunc setVisibleFunc random 

let internal handleKeyDownEvent (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (createFunc:unit->GameState<_>) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState<_>) :GameState<_> option =
    match state with
    | PlayState x -> 
        x |> handleKeyDownEventPlayState sumLocationsFunc setVisibleFunc worldSize random keyboardEvent
    | _ -> 
        state |> handleKeyDownEventDeadState createFunc keyboardEvent



