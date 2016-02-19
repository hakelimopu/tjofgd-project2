module EventHandler

open GameState
open CellLocation
open RenderCell
open MapCell
open MapObject

let private handleQuitEvent (quitEvent:SDLEvent.QuitEvent) (state:GameState) :GameState option =
    None

let rec updateActor (actorLocation:CellLocation) (actor:MapObject) (currentTurn:float<turn>) ((map,encounters):Map<CellLocation,MapCell>*CellLocation list) :(Map<CellLocation,MapCell>*CellLocation list)=
    if actor.CurrentTurn < currentTurn then
        //actor gets a turn!
        //TODO: does the actor still exist on the map at the original location?
        map, encounters
    else
        //nothing happens!
        map, encounters

let updateActors (currentTurn:float<turn>) (map:Map<CellLocation,MapCell>) :(Map<CellLocation,MapCell>*CellLocation list)=
    let actorLocations, _ = 
        map
        |> Map.partition (fun key value -> value.Object.IsSome)//TODO: this should already exist outside of the map, and map object should be removed from mapcell.
    let actors = 
        actorLocations
        |> Map.map (fun key value -> value.Object.Value)
    ((map,List.empty<CellLocation>), actors)
    ||> Map.fold (fun (currentMap:Map<CellLocation,MapCell>,encounters:CellLocation list) (location:CellLocation) (mapObject:MapObject) -> 
        updateActor location mapObject currentTurn (currentMap,encounters))

let moveBoat (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (delta:CellLocation) (state:PlayState) :GameState option =
    let playerLocation = state.MapGrid |> getPlayerLocation
    match playerLocation with
    | Some cellLocation -> 
        let boat = state.MapGrid.[cellLocation].Object.Value
        let nextLocation = delta |> sumLocationsFunc cellLocation
        let mapGrid, pcEncounter, npcEncounters =
            if state.MapGrid.ContainsKey nextLocation then
                //is the map grid occupied?
                if state.MapGrid.[nextLocation].Object.IsSome then
                    (state.MapGrid, nextLocation |> Some, List.empty)
                else
                    let updatedMapGrid, npcEncounters = 
                        state.MapGrid
                        |> setObject cellLocation None
                        |> setObject nextLocation (Some {boat with CurrentTurn = boat.CurrentTurn + 1.0<turn>})
                        |> updateVisibleFlags setVisibleFunc
                        |> updateActors (boat.CurrentTurn + 1.0<turn>)
                    (updatedMapGrid, None, npcEncounters)
            else
                (state.MapGrid, None, List.empty)
        {state with MapGrid = mapGrid; PCEncounter = pcEncounter} |> PlayState |> Some
    | None -> state |> PlayState |> Some
    
let private handleKeyDownEventPlayStateFreeMovement (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.F4 -> None
    | SDLKeyboard.ScanCode.Left   -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= -1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Right  -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Up     -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 0<cell>; Row= -1<cell>}
    | SDLKeyboard.ScanCode.Down   -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 0<cell>; Row= 1<cell>}
    | _                           -> state |> PlayState |> Some

let private handleKeyDownEventPlayStatePCEncounter (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Escape -> {state with PCEncounter=None} |> PlayState |> Some
    | _ -> state |> PlayState |> Some

let private handleKeyDownEventPlayStateNPCEncounters (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    state |> PlayState |> Some

let private handleKeyDownEventPlayState (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    (keyboardEvent, state)
    ||> match state with
        | FreeMovement     -> handleKeyDownEventPlayStateFreeMovement sumLocationsFunc setVisibleFunc 
        | HasPCEncounter   -> handleKeyDownEventPlayStatePCEncounter sumLocationsFunc setVisibleFunc 
        | HasNPCEncounters -> handleKeyDownEventPlayStateNPCEncounters sumLocationsFunc setVisibleFunc 

let private handleKeyDownEvent (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState) :GameState option =
    match state with
    | PlayState x -> x |> handleKeyDownEventPlayState sumLocationsFunc setVisibleFunc keyboardEvent

let MapViewX = 1<cell>
let MapViewY = 1<cell>
let MapViewWidth = 28<cell>
let MapViewHeight = 28<cell>

let mapViewCells =
    [0 .. (MapViewWidth/1<cell>)-1]
    |> Seq.map(fun column-> 
        [0 .. (MapViewHeight / 1<cell>)-1]
        |> Seq.map(fun row-> 
            ({Column=MapViewX + column * 1<cell>;Row=MapViewY + row * 1<cell>},{Column=column * 1<cell> - MapViewWidth / 2;Row=row * 1<cell> - MapViewHeight / 2})))
    |> Seq.reduce (Seq.append)
    |> Map.ofSeq


let private onIdlePlayState (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:PlayState) :GameState option =
    let playerLocation = 
        state.MapGrid
        |> getPlayerLocation
        |> Option.get
    let renderGrid = 
        (Map.empty<CellLocation,RenderCell>, mapViewCells)
        ||> Map.fold(fun renderGrid renderLocation mapDelta -> 
            let mapLocation = playerLocation |> sumLocationsFunc mapDelta
            let mapCell = state.MapGrid.TryFind mapLocation
            renderGrid
            |> Map.add renderLocation (mapCell |> renderCellForMapCell)) 
    {state with RenderGrid = renderGrid} |> PlayState |> Some

let private onIdle (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:GameState): GameState option =
    match state with
    | PlayState x -> x |> onIdlePlayState sumLocationsFunc

let handleEvent (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit quitDetails -> state |> handleQuitEvent quitDetails
    | SDLEvent.KeyDown keyDetails -> state |> handleKeyDownEvent sumLocationsFunc setVisibleFunc keyDetails
    | _ -> state |> onIdle sumLocationsFunc


