module KeyDownEvent

open CellLocation
open GameState
open EncounterChoice
open BoatMovement
open MapCell

let private handleKeyDownEventPlayState (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    (keyboardEvent, state)
    ||> match state with
        | FreeMovement     -> handleKeyDownEventPlayStateFreeMovement sumLocationsFunc setVisibleFunc 
        | HasPCEncounter   -> handleKeyDownEventPlayStatePCEncounter sumLocationsFunc setVisibleFunc 
        | HasNPCEncounters -> handleKeyDownEventPlayStateNPCEncounters sumLocationsFunc setVisibleFunc 

let internal handleKeyDownEvent (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState) :GameState option =
    match state with
    | PlayState x -> x |> handleKeyDownEventPlayState sumLocationsFunc setVisibleFunc keyboardEvent



