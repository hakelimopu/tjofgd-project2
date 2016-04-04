module QueryRepairEncounterChoice

open CellLocation
open GameState
open Random
open QueryRepairEncounterDetails
open MapObject

let private applyRepairs (location:CellLocation) (playState:PlayState<_>)  : GameState<_> option  =
    let _, maximumDamageRepaired, totalCost = 
        getRepairDetails location playState

    let boatLocation, boatTurn, boatProperties = 
        getBoat playState

    let boatProperties' =
        {boatProperties with Wallet = boatProperties.Wallet - totalCost; Hull = boatProperties.Hull + ((maximumDamageRepaired |> int) * 1<health>)}

    let actors' = 
        playState.Actors
        |> Map.add boatLocation {CurrentTurn = boatTurn; Detail = boatProperties' |> Boat}
    
    {playState with Encounters = None; Actors = actors'}|> PlayState |> Some

let internal applyQueryRepairEncounterChoice (randomFunc:RandomFunc) (detail:EncounterDetail) (location:CellLocation) (playState:PlayState<_>) : GameState<_> option = 
    match detail |> getEncounterResponse with
    | Common Confirm -> (location, playState) ||> applyRepairs
    | _       -> {playState with Encounters = None}|> PlayState |> Some
