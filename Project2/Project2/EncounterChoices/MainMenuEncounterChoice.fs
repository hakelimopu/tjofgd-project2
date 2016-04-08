module MainMenuEncounterChoice

open GameState
open GameMenuEncounterDetails

let openGameMenu  (playState: PlayState<_>) :PlayState<_> =
    {playState with Encounters = createGameMenuEncounterDetail playState |> PCEncounter |> Some}

let applyMainMenuPCEncounterChoice (detail:EncounterDetail) (playState: PlayState<_>) : GameState<_> option =
    match detail |> getEncounterResponse with
    | EncounterReponse.Menu MenuType.Game -> openGameMenu playState |> PlayState |> Some
    | _ -> {playState with Encounters = None}|> PlayState |> Some
    

