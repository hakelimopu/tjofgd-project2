module BoatMenuEncounterChoice

open GameState
open EncounterChoiceUtilities

let applyBoatMenuPCEncounterChoice (detail:EncounterDetail) (playState: PlayState<_>) : GameState<_> option =
    match detail |> getEncounterResponse with
    | _ -> playState|> clearEncounters |> PlayState |> Some


