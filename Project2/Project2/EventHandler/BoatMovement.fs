module BoatMovement

open CellLocation
open MapCell
open MapObject
open GameState
open EncounterHandler
open ActorUpdate

let moveBoat (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (delta:CellLocation) (state:PlayState) :GameState option =
    let playerLocation, _, _ = state |> getBoat
    let boat = state.Actors.[playerLocation]
    let nextLocation = delta |> sumLocationsFunc playerLocation
    if state.MapGrid.ContainsKey nextLocation then
        //is the map grid occupied?
        if state.Actors.ContainsKey(nextLocation) then
            {state with Encounters=(startPCEncounter nextLocation state)} |> PlayState |> Some
        else
            let updatedActors = 
                state.Actors
                |> setObject playerLocation None
                |> setObject nextLocation (Some {boat with CurrentTurn = boat.CurrentTurn + 1.0<turn>})
            let updatedMapGrid= 
                state
                |> updateVisibleFlags setVisibleFunc
            {state with MapGrid=updatedMapGrid;Actors=updatedActors}
            |> updateActors (boat.CurrentTurn + 1.0<turn>)
            |> PlayState
            |> Some
    else
        {state with Encounters = None} |> PlayState |> Some

let internal handleKeyDownEventPlayStateFreeMovement (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.F4 -> None
    | SDLKeyboard.ScanCode.Left   -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= -1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Right  -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Up     -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 0<cell>; Row= -1<cell>}
    | SDLKeyboard.ScanCode.Down   -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 0<cell>; Row= 1<cell>}
    | _                           -> state |> PlayState |> Some


