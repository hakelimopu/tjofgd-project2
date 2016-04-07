module MainMenuEncounterChoice

open GameState

let applyMainMenuPCEncounterChoice (detail:EncounterDetail) (playState: PlayState<_>) : GameState<_> option =
    match detail |> getEncounterResponse with
    | _ -> {playState with Encounters = None}|> PlayState |> Some
    

