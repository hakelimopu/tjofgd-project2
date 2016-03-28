module GridRenderer

open CellLocation
open GameState
open MapCell
open EncounterHandler
open RenderCell
open System
open MapObject
open MapTerrain

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

let private OutOfBoundsRenderCell = {Character=0xB0uy; Foreground=RenderCellColor.DarkGray; Background=RenderCellColor.Blue}
let private UnexploredRenderCell =  {Character=0x3Fuy; Foreground=RenderCellColor.Black;    Background=RenderCellColor.DarkGray}

let private TerrainRenderCells = 
    [(MapTerrain.Water       , {Character=0x7Euy; Foreground=RenderCellColor.Blue ; Background=RenderCellColor.BrightBlue });
     (MapTerrain.ShallowWater, {Character=0x20uy; Foreground=RenderCellColor.Blue ; Background=RenderCellColor.BrightBlue });
     (MapTerrain.Island      , {Character=0x1Euy; Foreground=RenderCellColor.Green; Background=RenderCellColor.BrightBlue });
     (MapTerrain.DeepWater   , {Character=0xF7uy; Foreground=RenderCellColor.Blue ; Background=RenderCellColor.BrightBlue })]
    |> Map.ofSeq

let private getActorRenderCell (detail:MapObject option) =
    match detail with
    | IsBoat        -> {Character=0xF1uy; Foreground=RenderCellColor.Brown       ; Background=RenderCellColor.BrightBlue }
    | IsStorm       -> {Character=0xF2uy; Foreground=RenderCellColor.BrightYellow; Background=RenderCellColor.BrightBlue }
    | IsPirate      -> {Character=0xF1uy; Foreground=RenderCellColor.Black       ; Background=RenderCellColor.BrightBlue }
    | IsMerfolk     -> {Character=0x02uy; Foreground=RenderCellColor.Magenta     ; Background=RenderCellColor.BrightBlue }
    | IsSeaMonster  -> {Character=0xEBuy; Foreground=RenderCellColor.DarkGray    ; Background=RenderCellColor.BrightBlue }
    | IsIsland      -> {Character=0x1Euy; Foreground=RenderCellColor.Green       ; Background=RenderCellColor.BrightBlue }
    | IsNothing     -> {Character=0x00uy; Foreground=RenderCellColor.Black       ; Background=RenderCellColor.Black      }


let renderCellForMapCell (actor:MapObject option) (mapCell:MapCell option) :RenderCell =
    match actor, mapCell with
    | Some _, Some x when     x.Visible -> actor |> getActorRenderCell
    | Some _, Some x when not x.Visible -> UnexploredRenderCell
    | None  , Some x when     x.Visible -> TerrainRenderCells.[mapCell.Value.Terrain]
    | None  , Some x when not x.Visible -> UnexploredRenderCell
    | _, _                              -> OutOfBoundsRenderCell

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


