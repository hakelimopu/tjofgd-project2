module EventHandler

open GameState
open SDLUtility

let private handleQuitEvent (quitEvent:SDLEvent.QuitEvent) (state:GameState) :GameState option =
    None

let private handleKeyDownEvent (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Escape -> None
    | _ -> Some state

let private onIdlePlayState (state:PlayState) :GameState option =
    let renderGrid = 
        state.MapGrid
        |> Map.fold (fun renderGrid cellLocation mapCell -> 
            renderGrid
            |> Map.add cellLocation (mapCell |> renderCellForMapCell)
            ) Map.empty<CellLocation,RenderCell>
    {state with RenderGrid = renderGrid} |> PlayState |> Some

let private onIdle (state:GameState): GameState option =
    match state with
    | PlayState x -> x |> onIdlePlayState

let handleEvent (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit x -> state |> handleQuitEvent x
    | SDLEvent.KeyDown x -> state |> handleKeyDownEvent x
    | _ -> state |> onIdle


