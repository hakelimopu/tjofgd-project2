module KeyDownEvent

open CellLocation
open GameState
open EncounterChoice
open BoatMovement
open MapCell
open Random
open EncounterChoiceUtilities
open EncounterChoice
open MainMenuEncounterDetails
//allowed controls:
//Up    (Controller Up)
//Down  (Controller Down)
//Left  (Controller Left)
//Right (Controller Right)
//Space (A)
//Enter (Y)
//Esc   (B)
//Tab   (X)
//F10   (Back)
//F2    (Start)

let internal handleFreeMovementCommand (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (state:PlayState<_>) (command:CommandType option) :GameState<_> option =
    let moveBoatFunc = moveBoat sumLocationsFunc setVisibleFunc worldSize random
    match command with
    | Some (CommandType.Menu Select) -> {state with Encounters = createMainMenuEncounterDetail state |> PCEncounter |> Some} |> PlayState |> Some
    | Some (CommandType.Move West)   -> state |> moveBoatFunc {Column= -1<cell>; Row=  0<cell>}
    | Some (CommandType.Move East)   -> state |> moveBoatFunc {Column=  1<cell>; Row=  0<cell>}
    | Some (CommandType.Move North)  -> state |> moveBoatFunc {Column=  0<cell>; Row= -1<cell>}
    | Some (CommandType.Move South)  -> state |> moveBoatFunc {Column=  0<cell>; Row=  1<cell>}
    | _                              -> state |> PlayState |> Some

let private freeMovementKeyboardTable =
    [
    (SDL.Keyboard.ScanCode.F10,     CommandType.Menu Select);
    (SDL.Keyboard.ScanCode.KeyPad4, CommandType.Move West);
    (SDL.Keyboard.ScanCode.Left,    CommandType.Move West);
    (SDL.Keyboard.ScanCode.KeyPad6, CommandType.Move East);
    (SDL.Keyboard.ScanCode.Right,   CommandType.Move East);
    (SDL.Keyboard.ScanCode.KeyPad2, CommandType.Move South);
    (SDL.Keyboard.ScanCode.Down,    CommandType.Move South);
    (SDL.Keyboard.ScanCode.KeyPad8, CommandType.Move North);
    (SDL.Keyboard.ScanCode.Up,      CommandType.Move North)]
    |> Map.ofSeq

let private encounterKeyboardTable =
    [(SDL.Keyboard.ScanCode.Space,  CommandType.Menu Select);
    (SDL.Keyboard.ScanCode.KeyPad2, CommandType.Menu Next);
    (SDL.Keyboard.ScanCode.Down,    CommandType.Menu Next);
    (SDL.Keyboard.ScanCode.KeyPad8, CommandType.Menu Previous);
    (SDL.Keyboard.ScanCode.Up,      CommandType.Menu Previous)]
    |> Map.ofSeq

let internal handlePCEncounterCommand (createFunc:unit->GameState<_>) (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:RandomFunc) (state:PlayState<_>)  (command:CommandType option):GameState<_> option =
    match command with
    | Some (CommandType.Menu Next)    -> {state with Encounters=(nextEncounterChoice state.Encounters)} |> PlayState |> Some
    | Some (CommandType.Menu Previous)-> {state with Encounters=(previousEncounterChoice state.Encounters)} |> PlayState |> Some
    | Some (CommandType.Menu Select)  -> state |> applyEncounterChoice createFunc random sumLocationsFunc setVisibleFunc
    | _                               -> state |> PlayState |> Some

let internal handleKeyDownEventPlayStateFreeMovement (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDL.Event.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    keyboardEvent.Keysym.Scancode
    |> freeMovementKeyboardTable.TryFind
    |> handleFreeMovementCommand sumLocationsFunc setVisibleFunc worldSize random state

let internal handleKeyDownEventPlayStateEncounter (createFunc:unit->GameState<_>) (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:RandomFunc) (keyboardEvent:SDL.Event.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    keyboardEvent.Keysym.Scancode
    |> encounterKeyboardTable.TryFind
    |> handlePCEncounterCommand createFunc sumLocationsFunc setVisibleFunc random state

let private handleKeyDownEventDeadState (createFunc:unit->GameState<_>) (keyboardEvent:SDL.Event.KeyboardEvent) (state:GameState<_>) :GameState<_> option =
    match keyboardEvent.Keysym.Scancode with
    | SDL.Keyboard.ScanCode.F2 -> createFunc() |> Some
    | _ -> state |> Some

let private handleKeyDownEventPlayState (createFunc:unit->GameState<_>) (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDL.Event.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    (keyboardEvent, state)
    ||> match state with
        | FreeMovement     -> handleKeyDownEventPlayStateFreeMovement  sumLocationsFunc setVisibleFunc worldSize random
        | HasEncounter   -> handleKeyDownEventPlayStateEncounter  createFunc sumLocationsFunc setVisibleFunc random

let internal handleKeyDownEvent (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (createFunc:unit->GameState<_>) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDL.Event.KeyboardEvent) (state:GameState<_>) :GameState<_> option =
    match state with
    | PlayState x -> 
        x |> handleKeyDownEventPlayState createFunc sumLocationsFunc setVisibleFunc worldSize random keyboardEvent
    | _ -> 
        state |> handleKeyDownEventDeadState createFunc keyboardEvent



