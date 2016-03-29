module QueryRepairEncounterChoice

open CellLocation
open GameState
open MapObject
open MapCreate
open Random

let internal applyQueryRepairEncounterChoice (randomFunc:RandomFunc) (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | _ -> {playState with Encounters = None}|> PlayState |> Some
