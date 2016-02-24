module EncounterChoice

open GameState
open MapCell
open MapObject
open CellLocation

let private nextEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) ->{details with CurrentChoice= (details.CurrentChoice + 1) % (details.Choices.Length)} |> PCEncounter |> Some
    | _ -> encounter

let private previousEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) ->{details with CurrentChoice= (details.CurrentChoice + details.Choices.Length - 1) % (details.Choices.Length)} |> PCEncounter |> Some
    | _ -> encounter

let private applyStormPCEncounterChoice (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (location:CellLocation) (playState:PlayState) : GameState option = 
        let playerLocation = playState.Actors |> getPlayerLocation |> Option.get
        let boat = playState.Actors.[playerLocation].Detail
        let boatProps = 
            match boat with
            | Boat props -> props
            | _ -> raise (new System.NotImplementedException())
        let storm = playState.Actors.[location].Detail
        let damage = 
            match storm with
            | Storm props -> props.Damage
            | _ -> raise (new System.NotImplementedException())
        let damagedBoat = {playState.Actors.[playerLocation] with Detail = ({boatProps with Hull=boatProps.Hull-damage} |> Boat)}
        let updatedActors = playState.Actors |> Map.remove playerLocation |> Map.add location damagedBoat
        {playState with Actors = updatedActors; MapGrid=playState.MapGrid |> updateVisibleFlags setVisibleFunc updatedActors; Encounters=None} |> PlayState |> Some

let private applyPCEncounterChoice (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (details:EncounterDetail) (playState:PlayState) : GameState option =
    match details.Type with
    | RanIntoStorm -> applyStormPCEncounterChoice setVisibleFunc details.Location playState

let private applyEncounterChoice (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (playState:PlayState) :GameState option =
    match playState.Encounters with
    | Some (PCEncounter details) -> playState |> applyPCEncounterChoice setVisibleFunc details
    | _ -> playState |> PlayState |> Some

let internal handleKeyDownEventPlayStatePCEncounter (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Down -> {state with Encounters=(nextEncounterChoice state.Encounters)} |> PlayState |> Some
    | SDLKeyboard.ScanCode.Up -> {state with Encounters=(previousEncounterChoice state.Encounters)} |> PlayState |> Some
    | SDLKeyboard.ScanCode.Return -> state |> applyEncounterChoice setVisibleFunc
    | _ -> state |> PlayState |> Some

let internal handleKeyDownEventPlayStateNPCEncounters (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    state |> PlayState |> Some



