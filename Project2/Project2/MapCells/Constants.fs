module Constants

open CellLocation
open MapTerrain

let WorldSize = {Column=240<cell>;Row=240<cell>}
let IslandDistance = 20<cell>

let mapLocations = 
    let unfoldColumns (rows:int<cell>) (columns:int<cell>) : (seq<CellLocation> * int<cell>) option =
        let unfoldRows (column:int<cell>) (rows:int<cell>) : (CellLocation * int<cell>) option =
            if rows = 0<cell> then
                None
            else
                Some ({Column = column; Row = (rows-1<cell>)}, rows-1<cell>)

        if columns = 0<cell> then
            None
        else
            Some ((rows) |> Seq.unfold (unfoldRows (columns-1<cell>)), columns-1<cell>)

    (WorldSize.Column)
    |> Seq.unfold (unfoldColumns WorldSize.Row)
    |> Seq.reduce (Seq.append)
    |> Set.ofSeq

let islandTemplate = 
    [(-3,-3,MapTerrain.DeepWater);
    (-3,-2,MapTerrain.DeepWater);
    (-3,-1,MapTerrain.Water);
    (-3, 0,MapTerrain.Water);
    (-3, 1,MapTerrain.Water);
    (-3, 2,MapTerrain.DeepWater);
    (-3, 3,MapTerrain.DeepWater);

    (-2,-3,MapTerrain.DeepWater);
    (-2,-2,MapTerrain.Water);
    (-2,-1,MapTerrain.Water);
    (-2, 0,MapTerrain.Water);
    (-2, 1,MapTerrain.Water);
    (-2, 2,MapTerrain.Water);
    (-2, 3,MapTerrain.DeepWater);
    
    (-1,-3,MapTerrain.Water);
    (-1,-2,MapTerrain.Water);
    (-1,-1,MapTerrain.ShallowWater);
    (-1, 0,MapTerrain.ShallowWater);
    (-1, 1,MapTerrain.ShallowWater);
    (-1, 2,MapTerrain.Water);
    (-1, 3,MapTerrain.Water);
    
    ( 0,-3,MapTerrain.Water);
    ( 0,-2,MapTerrain.Water);
    ( 0,-1,MapTerrain.ShallowWater);
    ( 0, 0,MapTerrain.Island);
    ( 0, 1,MapTerrain.ShallowWater);
    ( 0, 2,MapTerrain.Water);
    ( 0, 3,MapTerrain.Water);
    
    ( 3,-3,MapTerrain.DeepWater);
    ( 3,-2,MapTerrain.DeepWater);
    ( 3,-1,MapTerrain.Water);
    ( 3, 0,MapTerrain.Water);
    ( 3, 1,MapTerrain.Water);
    ( 3, 2,MapTerrain.DeepWater);
    ( 3, 3,MapTerrain.DeepWater);

    ( 2,-3,MapTerrain.DeepWater);
    ( 2,-2,MapTerrain.Water);
    ( 2,-1,MapTerrain.Water);
    ( 2, 0,MapTerrain.Water);
    ( 2, 1,MapTerrain.Water);
    ( 2, 2,MapTerrain.Water);
    ( 2, 3,MapTerrain.DeepWater);
      
    ( 1,-3,MapTerrain.Water);
    ( 1,-2,MapTerrain.Water);
    ( 1,-1,MapTerrain.ShallowWater);
    ( 1, 0,MapTerrain.ShallowWater);
    ( 1, 1,MapTerrain.ShallowWater);
    ( 1, 2,MapTerrain.Water);
    ( 1, 3,MapTerrain.Water)]
    |> List.map (fun (x,y,t) -> ({Column=x * 1<cell>;Row=y * 1<cell>},t))
    |> Map.ofList

let visibilityTemplate = 
    [(-2,-1);
    (-2, 0);
    (-2, 1);
    (-1,-2);
    (-1,-1);
    (-1, 0);
    (-1, 1);
    (-1, 2);
    ( 0,-2);
    ( 0,-1);
    ( 0, 0);
    ( 0, 1);
    ( 0, 2);
    ( 1,-2);
    ( 1,-1);
    ( 1, 0);
    ( 1, 1);
    ( 1, 2);
    ( 2,-1);
    ( 2, 0);
    ( 2, 1)]
    |> Seq.map(fun (x,y) -> {Column=x*1<cell>;Row=y*1<cell>})



