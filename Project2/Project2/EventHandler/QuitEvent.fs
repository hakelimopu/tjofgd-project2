module QuitEvent

open GameState

let internal handleQuitEvent (quitEvent:SDLEvent.QuitEvent) (state:GameState<_>) :GameState<_> option =
    None



