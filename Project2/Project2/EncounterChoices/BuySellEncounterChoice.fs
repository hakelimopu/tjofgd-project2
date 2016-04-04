module BuySellEncounterChoice

open CellLocation
open GameState
open Random

let internal applyBuySellEquipmentEncounterChoice (randomFunc:RandomFunc) (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | _       -> {playState with Encounters = None}|> PlayState |> Some
    //TODO: handle buying and selling!


