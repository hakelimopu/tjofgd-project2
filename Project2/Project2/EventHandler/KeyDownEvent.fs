module KeyDownEvent

open CellLocation
open GameState
open EncounterChoice
open BoatMovement
open MapCell

let private handleKeyDownEventPlayState (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:System.Random) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    (keyboardEvent, state)
    ||> match state with
        | FreeMovement     -> handleKeyDownEventPlayStateFreeMovement  sumLocationsFunc setVisibleFunc  random
        | HasPCEncounter   -> handleKeyDownEventPlayStatePCEncounter   sumLocationsFunc setVisibleFunc  random
        | HasNPCEncounters -> handleKeyDownEventPlayStateNPCEncounters sumLocationsFunc setVisibleFunc  random 

let internal handleKeyDownEvent (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:System.Random) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState) :GameState option =
    match state with
    | PlayState x -> x |> handleKeyDownEventPlayState sumLocationsFunc setVisibleFunc random keyboardEvent
    | _ -> state |> Some



