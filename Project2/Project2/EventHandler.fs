module EventHandler

open GameState
open SDLUtility

let private handleQuitEvent (quitEvent:SDLEvent.QuitEvent) (state:GameState) :GameState option =
    None

let private handleKeyDownEvent (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Escape -> None
    | _ -> Some state

let handleEvent (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit x -> state |> handleQuitEvent x
    | SDLEvent.KeyDown x -> state |> handleKeyDownEvent x
    | _ -> Some state


