module EventHandler

open GameState
open CellLocation
open RenderCell
open MapCell
open MapObject

let private handleQuitEvent (quitEvent:SDLEvent.QuitEvent) (state:GameState) :GameState option =
    None

let updateActors (currentTurn:float<turn>) (map:Map<CellLocation,MapCell>) :Map<CellLocation,MapCell> =
    //TODO - update all actors based on current turn of the boat
    map

let moveBoat (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (delta:CellLocation) (state:PlayState) :GameState option =
    let playerLocation = state.MapGrid |> getPlayerLocation
    match playerLocation with
    | Some cellLocation -> 
        let boat = state.MapGrid.[cellLocation].Object.Value
        let nextLocation = delta |> sumLocationsFunc cellLocation
        let mapGrid =
            if state.MapGrid.ContainsKey nextLocation then
                state.MapGrid
                |> setObject cellLocation None
                |> setObject nextLocation (Some {boat with CurrentTurn = boat.CurrentTurn + 1.0<turn>})
                |> updateVisibleFlags setVisibleFunc
                |> updateActors (boat.CurrentTurn + 1.0<turn>)
            else
                state.MapGrid
        {state with MapGrid = mapGrid} |> PlayState |> Some
    | None -> state |> PlayState |> Some
    
let private handleKeyDownEventPlayState (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Escape -> None
    | SDLKeyboard.ScanCode.Left   -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= -1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Right  -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Up     -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 0<cell>; Row= -1<cell>}
    | SDLKeyboard.ScanCode.Down   -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 0<cell>; Row= 1<cell>}
    | _                           -> state |> PlayState |> Some


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
        mapViewCells
        |> Map.fold(fun renderGrid renderLocation mapDelta -> 
            let mapLocation = playerLocation |> sumLocationsFunc mapDelta
            let mapCell = state.MapGrid.TryFind mapLocation
            renderGrid
            |> Map.add renderLocation (mapCell |> renderCellForMapCell)) Map.empty<CellLocation,RenderCell>
    {state with RenderGrid = renderGrid} |> PlayState |> Some

let private onIdle (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:GameState): GameState option =
    match state with
    | PlayState x -> x |> onIdlePlayState sumLocationsFunc

let handleEvent (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit x -> state |> handleQuitEvent x
    | SDLEvent.KeyDown x -> state |> handleKeyDownEvent sumLocationsFunc setVisibleFunc x
    | _ -> state |> onIdle sumLocationsFunc


