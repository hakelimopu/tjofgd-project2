module StormEncounterChoice

open CellLocation
open GameState
open MapCell
open MapObject

let internal applyStormEncounterChoice (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (location:CellLocation) (moveBoat:bool) (encounter:Encounters option) (playState:PlayState<_>) : GameState<_> option = 
    let playerLocation, _, boatProps = playState |> getBoat
    let _, storm = playState |> getStorm location
    let updatedBoatProperties = {boatProps with Hull=if storm.Damage> boatProps.Hull then 0<health> else boatProps.Hull-storm.Damage}
    let damagedBoat = {playState.Actors.[playerLocation] with Detail = (updatedBoatProperties |> Boat)}
    let updatedActors = 
        playState.Actors 
        |> Map.remove playerLocation
        |> Map.remove location
        |> Map.add (if moveBoat then location else playerLocation) damagedBoat
    let updatedPlayState = {playState with Actors = updatedActors; MapGrid=playState |> updateVisibleFlags sumLocationsFunc setVisibleFunc; Encounters=encounter}
    if updatedBoatProperties.Hull > 0<health> then
        updatedPlayState |> PlayState |> Some
    else
        updatedPlayState |> DeadState |> Some


