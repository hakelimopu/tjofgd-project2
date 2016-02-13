module EventHandler

open GameState

let private handleQuitEvent (quitEvent:SDLEvent.QuitEvent) (state:GameState) :GameState option =
    None

let moveBoat (delta:CellLocation) (state:PlayState) :GameState option =
    let playerLocation = state.MapGrid |> getPlayerLocation 
    match playerLocation with
    | Some cellLocation -> 
        let nextLocation = {Column=cellLocation.Column+delta.Column;Row=cellLocation.Row+delta.Row}
        let mapGrid =
            state.MapGrid
            |> setObject cellLocation None
            |> setObject nextLocation (Some MapObject.Boat)
        {state with MapGrid = mapGrid} |> PlayState |> Some
    | None -> state |> PlayState |> Some
    

let private handleKeyDownEventPlayState (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Escape -> None
    | SDLKeyboard.ScanCode.Left -> state |> moveBoat {Column= -1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Right -> state |> moveBoat {Column= 1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Up -> state |> moveBoat {Column= 0<cell>; Row= -1<cell>}
    | SDLKeyboard.ScanCode.Down -> state |> moveBoat {Column= 0<cell>; Row= 1<cell>}
    | _ -> state |> PlayState |> Some


let private handleKeyDownEvent (keyboardEvent:SDLEvent.KeyboardEvent) (state:GameState) :GameState option =
    match state with
    | PlayState x -> x |> handleKeyDownEventPlayState keyboardEvent

let MapViewX = 1<cell>
let MapViewY = 1<cell>
let MapViewWidth = 28<cell>
let MapViewHeight = 28<cell>

let private onIdlePlayState (state:PlayState) :GameState option =
    let playerLocation = 
        state.MapGrid
        |> getPlayerLocation
        |> Option.get
    let renderGrid = 
        [0 .. (MapViewWidth/1<cell>)-1]
        |> Seq.fold (fun outerRenderGrid mapColumn -> 
            [0 .. (MapViewHeight / 1<cell>)-1]
            |> Seq.fold(fun innerRenderGrid mapRow -> 
                let mapLocation = {Column=playerLocation.Column - MapViewWidth / 2  + mapColumn * 1<cell>;Row=playerLocation.Row - MapViewHeight/2  + mapRow * 1<cell>}
                let renderLocation = {Column=MapViewX + mapColumn * 1<cell>;Row=MapViewY + mapRow * 1<cell>}
                let mapCell = state.MapGrid.TryFind mapLocation
                innerRenderGrid
                |> Map.add renderLocation (mapCell |> renderCellForMapCell)
                ) outerRenderGrid) Map.empty<CellLocation,RenderCell>
    {state with RenderGrid = renderGrid} |> PlayState |> Some

let private onIdle (state:GameState): GameState option =
    match state with
    | PlayState x -> x |> onIdlePlayState

let handleEvent (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit x -> state |> handleQuitEvent x
    | SDLEvent.KeyDown x -> state |> handleKeyDownEvent x
    | _ -> state |> onIdle


