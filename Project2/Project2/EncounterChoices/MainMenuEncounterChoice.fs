module MainMenuEncounterChoice

open GameState
open GameMenuEncounterDetails
open BoatMenuEncounterDetails
open IslandMenuEncounterDetails

let openGameMenu  (playState: PlayState<_>) :PlayState<_> =
    {playState with Encounters = createGameMenuEncounterDetail playState |> PCEncounter |> Some}

let openBoatMenu  (playState: PlayState<_>) :PlayState<_> =
    {playState with Encounters = createBoatMenuEncounterDetail playState |> PCEncounter |> Some}

let openIslandMenu  (playState: PlayState<_>) :PlayState<_> =
    {playState with Encounters = createIslandMenuEncounterDetail playState |> PCEncounter |> Some}

let applyMainMenuPCEncounterChoice (detail:EncounterDetail) (playState: PlayState<_>) : GameState<_> option =
    match detail |> getEncounterResponse with
    | EncounterResponse.Menu MenuType.Game -> openGameMenu playState |> PlayState |> Some
    | EncounterResponse.Menu MenuType.Boat -> openBoatMenu playState |> PlayState |> Some
    | EncounterResponse.Menu MenuType.Island -> openIslandMenu playState |> PlayState |> Some
    | _ -> {playState with Encounters = None}|> PlayState |> Some
    

