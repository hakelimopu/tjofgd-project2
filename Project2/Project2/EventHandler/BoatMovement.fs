module BoatMovement

open CellLocation
open MapCell
open MapObject
open GameState
open EncounterHandler
open ActorUpdate

let generateStorm (flag:bool) (turn:float<turn>) (random:System.Random) (state:PlayState) : PlayState =
    if flag then
        let location = 
            state.MapGrid
            |> Map.toSeq
            |> Seq.map (fun (k,v)->k)
            |> Seq.sortBy(fun e-> random.Next())
            |> Seq.head
        if state.Actors.ContainsKey location then
            state
        else
            {state with Actors = state.Actors |> Map.add location {CurrentTurn=turn;Detail = Storm {Damage=1}}}
    else
        state

let moveBoat (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:System.Random) (delta:CellLocation) (state:PlayState) :GameState option =
    let playerLocation, boatTurn, boatProperties = state |> getBoat
    let nextLocation = delta |> sumLocationsFunc playerLocation
    if state.MapGrid.ContainsKey nextLocation then
        //is the map grid occupied?
        if state.Actors.ContainsKey(nextLocation) then
            //TODO: update boat turn, but after pc encounter?
            {state with Encounters=(startPCEncounter nextLocation state)} |> PlayState |> Some
        else
            let updatedBoatTurn = boatTurn + 1.0<turn>
            let spawnStorm, updateBoatProperties = 
                if updatedBoatTurn >= boatProperties.GenerateNextStorm then
                    true, {boatProperties with GenerateNextStorm = boatProperties.GenerateNextStorm + 5.0<turn>}
                else
                    false, boatProperties
            let updatedActors = 
                state.Actors
                |> setObject playerLocation None
                |> setObject nextLocation (Some {CurrentTurn = updatedBoatTurn;Detail = Boat updateBoatProperties})
            let updatedMapGrid= 
                state
                |> updateVisibleFlags setVisibleFunc
            {state with MapGrid=updatedMapGrid;Actors=updatedActors}
            |> generateStorm spawnStorm updatedBoatTurn random 
            |> updateActors sumLocationsFunc random updatedBoatTurn
            |> PlayState
            |> Some
    else
        {state with Encounters = None} |> PlayState |> Some

let internal handleKeyDownEventPlayStateFreeMovement (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (random:System.Random) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.F4 -> None
    | SDLKeyboard.ScanCode.Left   -> state |> moveBoat sumLocationsFunc setVisibleFunc random {Column= -1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Right  -> state |> moveBoat sumLocationsFunc setVisibleFunc random {Column= 1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Up     -> state |> moveBoat sumLocationsFunc setVisibleFunc random {Column= 0<cell>; Row= -1<cell>}
    | SDLKeyboard.ScanCode.Down   -> state |> moveBoat sumLocationsFunc setVisibleFunc random {Column= 0<cell>; Row= 1<cell>}
    | _                           -> state |> PlayState |> Some


