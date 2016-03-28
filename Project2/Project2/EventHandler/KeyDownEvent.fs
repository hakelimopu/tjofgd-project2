module KeyDownEvent

open CellLocation
open GameState
open EncounterChoice
open BoatMovement
open MapCell
open Random
open EncounterChoiceUtilities
open EncounterChoice

let internal handleFreeMovementCommand (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (state:PlayState<_>) (command:CommandType option) :GameState<_> option =
    let moveBoatFunc = moveBoat sumLocationsFunc setVisibleFunc worldSize random
    match command with
    | Some CommandType.Quit         -> None
    | Some (CommandType.Move West)  -> state |> moveBoatFunc {Column= -1<cell>; Row=  0<cell>}
    | Some (CommandType.Move East)  -> state |> moveBoatFunc {Column=  1<cell>; Row=  0<cell>}
    | Some (CommandType.Move North) -> state |> moveBoatFunc {Column=  0<cell>; Row= -1<cell>}
    | Some (CommandType.Move South) -> state |> moveBoatFunc {Column=  0<cell>; Row=  1<cell>}
    | _                             -> state |> PlayState |> Some

let private freeMovementKeyboardTable =
    [
    (SDLKeyboard.ScanCode.F4,      CommandType.Quit);
    (SDLKeyboard.ScanCode.KeyPad4, CommandType.Move West);
    (SDLKeyboard.ScanCode.Left,    CommandType.Move West);
    (SDLKeyboard.ScanCode.KeyPad6, CommandType.Move East);
    (SDLKeyboard.ScanCode.Right,   CommandType.Move East);
    (SDLKeyboard.ScanCode.KeyPad2, CommandType.Move South);
    (SDLKeyboard.ScanCode.Down,    CommandType.Move South);
    (SDLKeyboard.ScanCode.KeyPad8, CommandType.Move North);
    (SDLKeyboard.ScanCode.Up,      CommandType.Move North)]
    |> Map.ofSeq

let internal handleKeyDownEventPlayStateFreeMovement (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    keyboardEvent.Keysym.Scancode
    |> freeMovementKeyboardTable.TryFind
    |> handleFreeMovementCommand sumLocationsFunc setVisibleFunc worldSize random state

let internal handleKeyDownEventPlayStatePCEncounter  (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.KeyPad8
    | SDLKeyboard.ScanCode.Down   -> {state with Encounters=(nextEncounterChoice state.Encounters)} |> PlayState |> Some

    | SDLKeyboard.ScanCode.KeyPad2
    | SDLKeyboard.ScanCode.Up     -> {state with Encounters=(previousEncounterChoice state.Encounters)} |> PlayState |> Some

    | SDLKeyboard.ScanCode.KeyPadEnter
    | SDLKeyboard.ScanCode.Return -> state |> applyEncounterChoice sumLocationsFunc setVisibleFunc

    | _                           -> state |> PlayState |> Some

let internal handleKeyDownEventPlayStateNPCEncounters  (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Down -> {state with Encounters=(nextEncounterChoice state.Encounters)} |> PlayState |> Some
    | SDLKeyboard.ScanCode.Up -> {state with Encounters=(previousEncounterChoice state.Encounters)} |> PlayState |> Some
    | SDLKeyboard.ScanCode.Return -> state |> applyEncounterChoice sumLocationsFunc setVisibleFunc
    | _ -> state |> PlayState |> Some

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



