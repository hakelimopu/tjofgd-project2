module KeyDownEvent

open CellLocation
open GameState
open EncounterChoice
open BoatMovement
open MapCell
open Random
open EncounterChoiceUtilities
open EncounterChoice

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
    | Some CommandType.Quit         -> None
    | Some (CommandType.Move West)  -> state |> moveBoatFunc {Column= -1<cell>; Row=  0<cell>}
    | Some (CommandType.Move East)  -> state |> moveBoatFunc {Column=  1<cell>; Row=  0<cell>}
    | Some (CommandType.Move North) -> state |> moveBoatFunc {Column=  0<cell>; Row= -1<cell>}
    | Some (CommandType.Move South) -> state |> moveBoatFunc {Column=  0<cell>; Row=  1<cell>}
    | _                             -> state |> PlayState |> Some

let private freeMovementKeyboardTable =
    [
    (SDLKeyboard.ScanCode.F4,      CommandType.Quit);//TODO: get rid of me!
    (SDLKeyboard.ScanCode.KeyPad4, CommandType.Move West);
    (SDLKeyboard.ScanCode.Left,    CommandType.Move West);
    (SDLKeyboard.ScanCode.KeyPad6, CommandType.Move East);
    (SDLKeyboard.ScanCode.Right,   CommandType.Move East);
    (SDLKeyboard.ScanCode.KeyPad2, CommandType.Move South);
    (SDLKeyboard.ScanCode.Down,    CommandType.Move South);
    (SDLKeyboard.ScanCode.KeyPad8, CommandType.Move North);
    (SDLKeyboard.ScanCode.Up,      CommandType.Move North)]
    |> Map.ofSeq

let private encounterKeyboardTable =
    [(SDLKeyboard.ScanCode.Space,  CommandType.Menu Select);
    (SDLKeyboard.ScanCode.KeyPad2, CommandType.Menu Next);
    (SDLKeyboard.ScanCode.Down,    CommandType.Menu Next);
    (SDLKeyboard.ScanCode.KeyPad8, CommandType.Menu Previous);
    (SDLKeyboard.ScanCode.Up,      CommandType.Menu Previous)]
    |> Map.ofSeq

let internal handlePCEncounterCommand (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:RandomFunc) (state:PlayState<_>)  (command:CommandType option):GameState<_> option =
    match command with
    | Some (CommandType.Menu Next)    -> {state with Encounters=(nextEncounterChoice state.Encounters)} |> PlayState |> Some
    | Some (CommandType.Menu Previous)-> {state with Encounters=(previousEncounterChoice state.Encounters)} |> PlayState |> Some
    | Some (CommandType.Menu Select)  -> state |> applyEncounterChoice random sumLocationsFunc setVisibleFunc
    | _                               -> state |> PlayState |> Some

let internal handleKeyDownEventPlayStateFreeMovement (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    keyboardEvent.Keysym.Scancode
    |> freeMovementKeyboardTable.TryFind
    |> handleFreeMovementCommand sumLocationsFunc setVisibleFunc worldSize random state

let internal handleKeyDownEventPlayStateEncounter (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    keyboardEvent.Keysym.Scancode
    |> encounterKeyboardTable.TryFind
    |> handlePCEncounterCommand sumLocationsFunc setVisibleFunc random state

let private handleKeyDownEventDeadState (createFunc:unit->GameState<_>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState<_>) :GameState<_> option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.F2 -> createFunc() |> Some
    | _ -> state |> Some

let private handleKeyDownEventPlayState (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState<_>) :GameState<_> option =
    (keyboardEvent, state)
    ||> match state with
        | FreeMovement     -> handleKeyDownEventPlayStateFreeMovement  sumLocationsFunc setVisibleFunc worldSize random
        | HasEncounter   -> handleKeyDownEventPlayStateEncounter   sumLocationsFunc setVisibleFunc random

let internal handleKeyDownEvent (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (createFunc:unit->GameState<_>) (worldSize:CellLocation) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState<_>) :GameState<_> option =
    match state with
    | PlayState x -> 
        x |> handleKeyDownEventPlayState sumLocationsFunc setVisibleFunc worldSize random keyboardEvent
    | _ -> 
        state |> handleKeyDownEventDeadState createFunc keyboardEvent



