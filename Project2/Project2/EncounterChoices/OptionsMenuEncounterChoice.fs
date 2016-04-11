module OptionsMenuEncounterChoice

open GameState
open EncounterChoiceUtilities

let applyOptionsMenuPCEncounterChoice (detail:EncounterDetail) (playState: PlayState<_>) : GameState<_> option =
    match detail |> getEncounterResponse with
    | _ -> playState|> clearEncounters |> PlayState |> Some


