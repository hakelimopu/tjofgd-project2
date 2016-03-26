module IdleHandler

open CellLocation
open GameState
open MapCell
open EncounterHandler
open RenderCell
open System
open MapObject

let MapViewX = 0<cell>
let MapViewY = 0<cell>
let MapViewWidth = 30<cell>
let MapViewHeight = 30<cell>
let StatsViewX = MapViewX + MapViewWidth
let StatsViewY = MapViewY
let StatsViewWidth = 10<cell>
let StatsViewHeight = MapViewHeight

let mapViewCells =
    [0 .. (MapViewWidth/1<cell>)-1]
    |> Seq.map(fun column-> 
        [0 .. (MapViewHeight / 1<cell>)-1]
        |> Seq.map(fun row-> 
            ({Column=MapViewX + column * 1<cell>;Row=MapViewY + row * 1<cell>},{Column=column * 1<cell> - MapViewWidth / 2;Row=row * 1<cell> - MapViewHeight / 2})))
    |> Seq.reduce (Seq.append)
    |> Map.ofSeq

let private renderStats (vectorToLocationFunc: VectorToLocationFunc) (state:PlayState<_>) (grid:CellMap<RenderCell>):CellMap<RenderCell> =
    let playerLocation, _, boatProps = state |> getBoat
    let questText = 
        if boatProps.Quest.IsSome then 
            let quest = boatProps.Quest |> Option.get
            let _,island = state |> getIsland quest.Destination
            island.Name
        else 
            ""
    let questDistance =
        if boatProps.Quest.IsSome then 
            let quest = boatProps.Quest |> Option.get
            let magnitude, _ = vectorToLocationFunc playerLocation quest.Destination
            magnitude / 1.0<cell> 
            |> sprintf "%.2f"
        else 
            ""
    grid
    |> writeText {Column=StatsViewX;Row=StatsViewY} RenderCellColor.Black RenderCellColor.Black ("          ")
    |> writeText {Column=StatsViewX;Row=StatsViewY} RenderCellColor.Brown RenderCellColor.Black (sprintf "Hull:%2i/%2i" (boatProps.Hull/1<health>) (boatProps.MaximumHull/1<health>))
    |> writeText {Column=StatsViewX;Row=(StatsViewY+1<cell>)} RenderCellColor.Black RenderCellColor.Black ("          ")
    |> writeText {Column=StatsViewX;Row=(StatsViewY+1<cell>)} RenderCellColor.BrightYellow RenderCellColor.Black (sprintf "$%9.2f" (boatProps.Wallet/1.0<currency>))
    |> writeText {Column=StatsViewX;Row=(StatsViewY+2<cell>)} RenderCellColor.White RenderCellColor.Black (sprintf "          ")
    |> writeText {Column=StatsViewX;Row=(StatsViewY+3<cell>)} RenderCellColor.White RenderCellColor.Black (sprintf "          ")
    |> writeText {Column=StatsViewX;Row=(StatsViewY+2<cell>)} RenderCellColor.White RenderCellColor.Black (sprintf "%s" questText)
    |> writeText {Column=StatsViewX;Row=(StatsViewY+3<cell>)} RenderCellColor.White RenderCellColor.Black (sprintf "%s" questDistance)

let private renderFolder (sumLocationsFunc:SumLocationsFunc) (playerLocation:CellLocation) (state:PlayState<CellMap<RenderCell>>) (renderGrid:CellMap<RenderCell>) (renderLocation:CellLocation) (mapDelta:CellLocation) :CellMap<RenderCell> =
    let mapLocation = playerLocation |> sumLocationsFunc mapDelta
    let mapCell = state.MapGrid.TryFind mapLocation
    let actor = state.Actors.TryFind mapLocation
    renderGrid
    |> Map.add renderLocation (renderCellForMapCell actor mapCell)

let gridRenderer (vectorToLocationFunc: VectorToLocationFunc) (sumLocationsFunc:SumLocationsFunc) (state:PlayState<CellMap<RenderCell>>) :PlayState<CellMap<RenderCell>> =
    let playerLocation = state |> getBoatLocation
    let renderGrid :CellMap<RenderCell>= 
        (state.RenderData, mapViewCells)
        ||> Map.fold (renderFolder sumLocationsFunc playerLocation state) 
        |> renderEncounter state
        |> renderStats vectorToLocationFunc state
    {state with RenderData = renderGrid}

type GridRendererFunc<'TRender> = SumLocationsFunc -> PlayState<'TRender> -> PlayState<'TRender>

let private onIdlePlayState<'TRender> (renderer:GridRendererFunc<'TRender>) (sumLocationsFunc:SumLocationsFunc) (state:PlayState<'TRender>) :GameState<'TRender> option =
    state
    |> renderer sumLocationsFunc 
    |> PlayState 
    |> Some

//TODO: make this not a copypasta!
let private onIdleDeadState<'TRender> (renderer:GridRendererFunc<'TRender>) (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:PlayState<'TRender>) :GameState<'TRender> option =
    state
    |> renderer sumLocationsFunc 
    |> DeadState 
    |> Some

let internal onIdle<'TRender> (renderer:GridRendererFunc<'TRender>) (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:GameState<'TRender>): GameState<'TRender> option =
    match state with
    | PlayState x -> 
        x |> onIdlePlayState<'TRender> renderer sumLocationsFunc
    | DeadState x -> 
        x |> onIdleDeadState<'TRender> renderer sumLocationsFunc


