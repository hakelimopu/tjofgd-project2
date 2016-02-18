module Constants

open CellLocation
open MapTerrain

let WorldSize = {Column=256<cell>;Row=256<cell>}
let IslandDistance = 20<cell>

let mapLocations = 
    [0 .. (WorldSize.Column/1<cell>)-1]
    |> Seq.map(fun column-> 
        [0 .. (WorldSize.Row / 1<cell>)-1]
        |> Seq.map(fun row-> 
            {Column=column * 1<cell>;Row=row * 1<cell>}))
    |> Seq.reduce (Seq.append)

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



