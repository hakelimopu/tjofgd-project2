module QuitEvent

open GameState

let internal handleQuitEvent (quitEvent:SDL.Event.QuitEvent) (state:GameState<_>) :GameState<_> option =
    None



