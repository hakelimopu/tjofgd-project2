module EventHandler

open GameState
open CellLocation
open RenderCell
open MapCell
open MapObject
open System.Text

let private handleQuitEvent (quitEvent:SDLEvent.QuitEvent) (state:GameState) :GameState option =
    None

let rec updateActor (actorLocation:CellLocation) (actor:MapObject) (currentTurn:float<turn>) (playState:PlayState) :PlayState=
    if actor.CurrentTurn < currentTurn then
        //actor gets a turn!
        //TODO: does the actor still exist on the map at the original location?
        playState
    else
        //nothing happens!
        playState

let updateActors (currentTurn:float<turn>) (playState:PlayState) :PlayState=
    (playState, playState.Actors)
    ||> Map.fold (fun currentState location actor -> 
        updateActor location actor currentTurn currentState)

let private startPCEncounter (location:CellLocation) (state:PlayState) :Encounters option =
    let actor = state.Actors.[location]
    match actor.Detail with
    | Storm stormProperties -> {Location=location;Title="Storm!";Type=RanIntoStorm;Message=["You have run into a storm,";"and it has damaged your boat!"];Choices=["OK"];CurrentChoice=0} |> PCEncounter |> Some
    | _ -> None

let moveBoat (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (delta:CellLocation) (state:PlayState) :GameState option =
    let playerLocation = state.Actors |> getPlayerLocation
    match playerLocation with
    | Some cellLocation -> 
        let boat = state.Actors.[cellLocation]
        let nextLocation = delta |> sumLocationsFunc cellLocation
        if state.MapGrid.ContainsKey nextLocation then
            //is the map grid occupied?
            if state.Actors.ContainsKey(nextLocation) then
                {state with Encounters=(startPCEncounter nextLocation state)} |> PlayState |> Some
            else
                let updatedActors = 
                    state.Actors
                    |> setObject cellLocation None
                    |> setObject nextLocation (Some {boat with CurrentTurn = boat.CurrentTurn + 1.0<turn>})
                let updatedMapGrid= 
                    state.MapGrid
                    |> updateVisibleFlags setVisibleFunc updatedActors
                {state with MapGrid=updatedMapGrid;Actors=updatedActors}
                |> updateActors (boat.CurrentTurn + 1.0<turn>)
                |> PlayState
                |> Some
        else
            {state with Encounters = None} |> PlayState |> Some
    | None -> state |> PlayState |> Some
    
let private handleKeyDownEventPlayStateFreeMovement (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.F4 -> None
    | SDLKeyboard.ScanCode.Left   -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= -1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Right  -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 1<cell>; Row=0<cell>}
    | SDLKeyboard.ScanCode.Up     -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 0<cell>; Row= -1<cell>}
    | SDLKeyboard.ScanCode.Down   -> state |> moveBoat sumLocationsFunc setVisibleFunc {Column= 0<cell>; Row= 1<cell>}
    | _                           -> state |> PlayState |> Some

let private nextEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) ->{details with CurrentChoice= (details.CurrentChoice + 1) % (details.Choices.Length)} |> PCEncounter |> Some
    | _ -> encounter

let private previousEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) ->{details with CurrentChoice= (details.CurrentChoice + details.Choices.Length - 1) % (details.Choices.Length)} |> PCEncounter |> Some
    | _ -> encounter

let private applyPCEncounterChoice (details:EncounterDetail) (playState:PlayState) : GameState option =
    match details.Type with
    | RanIntoStorm ->
        let playerLocation = playState.Actors |> getPlayerLocation |> Option.get
        let boat = playState.Actors.[playerLocation].Detail
        let boatProps = 
            match boat with
            | Boat props -> props
            | _ -> raise (new System.NotImplementedException())
        let storm = playState.Actors.[details.Location].Detail
        let damage = 
            match storm with
            | Storm props -> props.Damage
            | _ -> raise (new System.NotImplementedException())
        let damagedBoat = {playState.Actors.[playerLocation] with Detail = ({boatProps with Hull=boatProps.Hull-damage} |> Boat)}
        //TODO: update visibility flags!
        {playState with Actors = playState.Actors |> Map.remove playerLocation |> Map.add details.Location damagedBoat; Encounters=None} |> PlayState |> Some

let private applyEncounterChoice (playState:PlayState) :GameState option =
    match playState.Encounters with
    | Some (PCEncounter details) -> playState |> applyPCEncounterChoice details
    | _ -> playState |> PlayState |> Some

let private handleKeyDownEventPlayStatePCEncounter (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (keyboardEvent:SDLEvent.KeyboardEvent) (state:PlayState) :GameState option =
    match keyboardEvent.Keysym.Scancode with
    | SDLKeyboard.ScanCode.Down -> {state with Encounters=(nextEncounterChoice state.Encounters)} |> PlayState |> Some
    | SDLKeyboard.ScanCode.Up -> {state with Encounters=(previousEncounterChoice state.Encounters)} |> PlayState |> Some
    | SDLKeyboard.ScanCode.Return -> state |> applyEncounterChoice
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

let MapViewX = 0<cell>
let MapViewY = 0<cell>
let MapViewWidth = 30<cell>
let MapViewHeight = 30<cell>

let mapViewCells =
    [0 .. (MapViewWidth/1<cell>)-1]
    |> Seq.map(fun column-> 
        [0 .. (MapViewHeight / 1<cell>)-1]
        |> Seq.map(fun row-> 
            ({Column=MapViewX + column * 1<cell>;Row=MapViewY + row * 1<cell>},{Column=column * 1<cell> - MapViewWidth / 2;Row=row * 1<cell> - MapViewHeight / 2})))
    |> Seq.reduce (Seq.append)
    |> Map.ofSeq

let private drawText (location:CellLocation) (foreground:RenderCellColor) (background:RenderCellColor) (text:string) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let bytes = Encoding.ASCII.GetBytes(text)
    ((renderGrid,location),bytes)
    ||> Seq.fold(fun (grid,position) character->
        (grid |> Map.add position {Character=character;Foreground=foreground;Background=background},{position with Column=position.Column+1<cell>}))
    |> fst

let private renderPCEncounter (details:EncounterDetail)  (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let g,p = 
        ((renderGrid |> drawText {Column=0<cell>;Row=0<cell>} RenderCellColor.Blue RenderCellColor.Black details.Title,{Column=0<cell>;Row=1<cell>}), details.Message)
        ||> List.fold (fun (grid,position) line -> (grid |> drawText position RenderCellColor.White RenderCellColor.Black line ,{position with Row=position.Row+1<cell>}))
    let finalGrid, _, _ =
        ((g,p,0),details.Choices)
        ||> List.fold(fun (grid,position,counter) choice -> 
            ((if counter=details.CurrentChoice then (grid |> drawText position RenderCellColor.Cyan RenderCellColor.BrightYellow choice) else (grid |> drawText position RenderCellColor.Cyan RenderCellColor.DarkGray choice)),{position with Row=position.Row+1<cell>},counter+1))
    finalGrid

let private renderEncounter (state:PlayState) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    match state.Encounters with
    | Some (PCEncounter details) -> renderPCEncounter details renderGrid
    | _ -> renderGrid

let private onIdlePlayState (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:PlayState) :GameState option =
    let playerLocation = 
        state.Actors
        |> getPlayerLocation
        |> Option.get
    let renderGrid = 
        (state.RenderGrid, mapViewCells)
        ||> Map.fold(fun renderGrid renderLocation mapDelta -> 
            let mapLocation = playerLocation |> sumLocationsFunc mapDelta
            let mapCell = state.MapGrid.TryFind mapLocation
            let actor = state.Actors.TryFind mapLocation
            renderGrid
            |> Map.add renderLocation (renderCellForMapCell actor mapCell)) 
        |> renderEncounter state
    {state with RenderGrid = renderGrid} |> PlayState |> Some

let private onIdle (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:GameState): GameState option =
    match state with
    | PlayState x -> x |> onIdlePlayState sumLocationsFunc

let handleEvent (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit quitDetails -> state |> handleQuitEvent quitDetails
    | SDLEvent.KeyDown keyDetails -> state |> handleKeyDownEvent sumLocationsFunc setVisibleFunc keyDetails
    | _ -> state |> onIdle sumLocationsFunc


