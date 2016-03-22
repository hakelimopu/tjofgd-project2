﻿module EncounterChoice

open GameState
open MapCell
open MapObject
open CellLocation
open Random

let private nextEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) ->{details with CurrentChoice= (details.CurrentChoice + 1) % (details.Choices.Length)} |> PCEncounter |> Some
    | Some (NPCEncounters (head :: tail)) -> {head with CurrentChoice= (head.CurrentChoice + 1) % (head.Choices.Length)} :: tail |> NPCEncounters |> Some
    | _ -> encounter

let private previousEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) ->{details with CurrentChoice= (details.CurrentChoice + details.Choices.Length - 1) % (details.Choices.Length)} |> PCEncounter |> Some
    | Some (NPCEncounters (head :: tail)) -> {head with CurrentChoice= (head.CurrentChoice + head.Choices.Length - 1) % (head.Choices.Length)} :: tail |> NPCEncounters |> Some
    | _ -> encounter


let private repairBoat (playState:PlayState) : PlayState =
    let location, turns, boatProps = playState |> getBoat
    let repairedBoat = {playState.Actors.[location] with Detail = ({boatProps with Hull=boatProps.MaximumHull} |> Boat); CurrentTurn=turns + (((boatProps.MaximumHull-boatProps.Hull)|> float) * 1.0<turn>)}
    {playState with Actors = (playState.Actors |> Map.add location repairedBoat)}

let private applyDockPCEncounterChoice (detail:EncounterDetail) (playState:PlayState) : GameState option = 
    match detail |> getEncounterResponse with
    | Repair -> {(playState |> repairBoat) with Encounters=None} |> PlayState |> Some
    | _ -> {playState with Encounters=None} |> PlayState |> Some
    

let private applyStormEncounterChoice (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (location:CellLocation) (moveBoat:bool) (encounter:Encounters option) (playState:PlayState) : GameState option = 
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


let private applyPCEncounterChoice (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (details:EncounterDetail) (playState:PlayState) : GameState option =
    match details.Type with
    | RanIntoStorm -> 
        applyStormEncounterChoice sumLocationsFunc setVisibleFunc details.Location true None playState
    | DockedWithIsland -> applyDockPCEncounterChoice details playState

let private applyNPCEncounterChoice (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (head:EncounterDetail) (tail:EncounterDetail list) (playState:PlayState): GameState option =
    let nextEncounter =
        match tail with
        | [] -> None
        | _ -> tail |> NPCEncounters |> Some
    match head.Type with 
    | RanIntoStorm -> 
        applyStormEncounterChoice sumLocationsFunc setVisibleFunc head.Location false nextEncounter playState
    | _ -> raise (new System.NotImplementedException("This encounter is not implemented for NPC encounters!"))

let private applyEncounterChoice (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (playState:PlayState) :GameState option =
    match playState.Encounters with
    | Some (PCEncounter details) -> 
        (details, playState) ||> applyPCEncounterChoice sumLocationsFunc setVisibleFunc
    | Some (NPCEncounters (head::tail)) -> 
        (head, tail, playState) |||> applyNPCEncounterChoice sumLocationsFunc setVisibleFunc
    | _ -> playState |> PlayState |> Some

let internal handleKeyDownEventPlayStatePCEncounter  (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.KeyPad8
    | SDLKeyboard.ScanCode.Down   -> {state with Encounters=(nextEncounterChoice state.Encounters)} |> PlayState |> Some

    | SDLKeyboard.ScanCode.KeyPad2
    | SDLKeyboard.ScanCode.Up     -> {state with Encounters=(previousEncounterChoice state.Encounters)} |> PlayState |> Some

    | SDLKeyboard.ScanCode.KeyPadEnter
    | SDLKeyboard.ScanCode.Return -> state |> applyEncounterChoice sumLocationsFunc setVisibleFunc

    | _                           -> state |> PlayState |> Some

let internal handleKeyDownEventPlayStateNPCEncounters  (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:RandomFunc) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Down -> {state with Encounters=(nextEncounterChoice state.Encounters)} |> PlayState |> Some
    | SDLKeyboard.ScanCode.Up -> {state with Encounters=(previousEncounterChoice state.Encounters)} |> PlayState |> Some
    | SDLKeyboard.ScanCode.Return -> state |> applyEncounterChoice sumLocationsFunc setVisibleFunc
    | _ -> state |> PlayState |> Some



