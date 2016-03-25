module EncounterDetailUtilities

open GameState

let ``always include choice`` (playState:PlayState<_>) :bool = true

let filterChoice (playState:PlayState<_>) (choice, func) =
    playState |> func



