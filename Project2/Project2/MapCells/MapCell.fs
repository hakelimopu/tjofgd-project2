module MapCell

open CellLocation
open MapTerrain
open MapObject

type MapCell =
    {Terrain:MapTerrain;
    Visible:bool}

let setTerrain (cellLocation:CellLocation) (mapTerrain:MapTerrain) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    let originalCell = 
        cellMap.TryFind(cellLocation)
    let newCell =
        if originalCell.IsSome then
            {originalCell.Value with Terrain=mapTerrain}
        else
            {Terrain=mapTerrain;Visible=false}
    cellMap
    |> Map.add cellLocation newCell

let setTerrainWrapped (worldSize:CellLocation) (cellLocation:CellLocation) (mapTerrain:MapTerrain) (cellMap:CellMap<MapCell>) :CellMap<MapCell> =
    setTerrain (cellLocation |> wrapLocation worldSize) mapTerrain cellMap

let setVisible (cellLocation:CellLocation) (cellMap:CellMap<MapCell>) :CellMap<MapCell> = 
    match cellMap.TryFind(cellLocation) with
    | None -> cellMap
    | Some mapCell -> 
        cellMap
        |> Map.add cellLocation {mapCell with Visible=true}

let setVisibleWrapped (worldSize:CellLocation) (cellLocation:CellLocation) (cellMap:CellMap<MapCell>) :CellMap<MapCell> = 
    setVisible (cellLocation |> wrapLocation worldSize) cellMap
    
let setObject (cellLocation:CellLocation) (mapObject:MapObject option) (actors:CellMap<MapObject>) :CellMap<MapObject> =
    match mapObject with
    | Some actor ->
        actors
        |> Map.add cellLocation actor
    | _ -> 
        actors 
        |> Map.remove cellLocation

let setObjectWrapped (worldSize:CellLocation) (cellLocation:CellLocation) (mapObject:MapObject option) (actors:CellMap<MapObject>) :CellMap<MapObject> =
    setObject (cellLocation |> wrapLocation worldSize) mapObject actors


