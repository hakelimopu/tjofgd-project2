module IslandMenuEncounterChoice

open GameState
open EncounterChoiceUtilities

let applyIslandMenuPCEncounterChoice (detail:EncounterDetail) (playState: PlayState<_>) : GameState<_> option =
    match detail |> getEncounterResponse with
    | _ -> playState |> clearEncounters |> PlayState |> Some
