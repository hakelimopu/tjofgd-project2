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

let private renderStats (state:PlayState) (grid:CellMap<RenderCell>):CellMap<RenderCell> =
    let _, _, boatProps = state |> getBoat
    grid
    |> writeText {Column=StatsViewX;Row=StatsViewY} RenderCellColor.Black RenderCellColor.Black ("          ")
    |> writeText {Column=StatsViewX;Row=StatsViewY} RenderCellColor.Brown RenderCellColor.Black (sprintf "Hull:%2i/%2i" boatProps.Hull boatProps.MaximumHull)
    

let private onIdlePlayState (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:PlayState) :GameState option =
    let playerLocation, _, _ = state |> getBoat
    let renderGrid = 
        (state.RenderGrid, mapViewCells)
        ||> Map.fold(fun renderGrid renderLocation mapDelta -> 
            let mapLocation = playerLocation |> sumLocationsFunc mapDelta
            let mapCell = state.MapGrid.TryFind mapLocation
            let actor = state.Actors.TryFind mapLocation
            renderGrid
            |> Map.add renderLocation (renderCellForMapCell actor mapCell)) 
        |> renderEncounter state
        |> renderStats state
    {state with RenderGrid = renderGrid} |> PlayState |> Some

let internal onIdle (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:GameState): GameState option =
    match state with
    | PlayState x -> x |> onIdlePlayState sumLocationsFunc


