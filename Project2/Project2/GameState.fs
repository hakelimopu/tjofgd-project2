module GameState

open CellLocation
open RenderCell
open MapCell
open MapTerrain
open MapObject

let OutOfBoundsRenderCell = {Character=0xB0uy;Foreground=RenderCellColor.DarkGray;Background=RenderCellColor.Blue}
let UnexploredRenderCell = {Character=0x3Fuy;Foreground=RenderCellColor.Black;Background=RenderCellColor.DarkGray}

let TerrainRenderCells = 
    [(MapTerrain.Water, {Character=0x7Euy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue});
    (MapTerrain.ShallowWater, {Character=0x20uy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue});
    (MapTerrain.Island, {Character=0x1Euy;Foreground=RenderCellColor.Green;Background=RenderCellColor.BrightBlue});
    (MapTerrain.DeepWater, {Character=0xF7uy;Foreground=RenderCellColor.Blue;Background=RenderCellColor.BrightBlue})]
    |> Map.ofSeq

let ObjectRenderCells (detail:MapObject option) =
    match detail with
    | Boat        -> {Character=0xF1uy;Foreground=RenderCellColor.Brown;Background=RenderCellColor.BrightBlue}
    | Storm       -> {Character=0xF2uy;Foreground=RenderCellColor.BrightYellow;Background=RenderCellColor.BrightBlue}
    | Pirate      -> {Character=0xF1uy;Foreground=RenderCellColor.Black;Background=RenderCellColor.BrightBlue}
    | Merfolk     -> {Character=0x02uy;Foreground=RenderCellColor.Magenta;Background=RenderCellColor.BrightBlue}
    | SeaMonster  -> {Character=0xEBuy;Foreground=RenderCellColor.DarkGray;Background=RenderCellColor.BrightBlue}
    | Nothing     -> {Character=0x00uy;Foreground=RenderCellColor.Black;Background=RenderCellColor.Black}

let renderCellForMapCell (mapCell:MapCell option) :RenderCell =
    if mapCell.IsSome then
        match mapCell.Value.Visible, mapCell.Value.Object with
        | true, Some mapObject -> ObjectRenderCells (Some mapObject)
        | true, None -> TerrainRenderCells.[mapCell.Value.Terrain]
        | false, _ -> UnexploredRenderCell
    else
        OutOfBoundsRenderCell

type PlayState =
    {RenderGrid:CellMap<RenderCell>;
    MapGrid:CellMap<MapCell>}

type GameState = 
    | PlayState of PlayState

