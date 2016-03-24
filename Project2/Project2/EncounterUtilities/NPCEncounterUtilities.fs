module NPCEncounterUtilities

open GameState
open CellLocation
open EncounterDetails

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute>]
exception IncompatibleEncounterType

let addNPCEncounter (encounterDetail:EncounterDetail) (playState:PlayState<_>) :PlayState<_> =
    match playState.Encounters with
    | None                            -> {playState with Encounters = Some (NPCEncounters [encounterDetail])}
    | Some (NPCEncounters detailList) -> {playState with Encounters = Some (NPCEncounters (detailList @ [encounterDetail]))}
    | Some (PCEncounter detail)       -> raise IncompatibleEncounterType

let addNPCStormEncounter (actorLocation:CellLocation) (playState:PlayState<_>) :PlayState<_> =
    (actorLocation |> createStormEncounterDetail,
     playState)
    ||> addNPCEncounter
