module CellLocation

open SDLUtility

[<Measure>] type cell

type CellLocation =
    {Column:int<cell>;Row:int<cell>}

let sumLocations (first:CellLocation) (second:CellLocation) :CellLocation =
    {Column=first.Column+second.Column;Row=first.Row+second.Row}

let wrapDimension dimensionSize dimensionValue =
    let newDimensionValue = dimensionValue % dimensionSize
    if newDimensionValue < 0<cell> then newDimensionValue + dimensionSize else newDimensionValue

let wrapLocation (worldSize:CellLocation) (location:CellLocation) = 
    {Column=location.Column |> wrapDimension worldSize.Column; Row =location.Row |> wrapDimension worldSize.Row}

let sumLocationsWrapped (worldSize:CellLocation) (first:CellLocation) (second:CellLocation) :CellLocation =
    sumLocations first second
    |> wrapLocation worldSize

let pixelsPerColumn = 8<px/cell>
let pixelsPerRow = 8<px/cell>

type CellMap<'T> = Map<CellLocation,'T>

let distanceFormulaTest (maximum:int<cell>) (first:CellLocation) (second:CellLocation) :bool =
    let deltaX = second.Column - first.Column
    let deltaY = second.Row - first.Row
    (deltaX*deltaX+deltaY*deltaY) > maximum * maximum

let distanceFormulaTestWrapped (worldSize:CellLocation) (maximum:int<cell>) (first:CellLocation) (second:CellLocation) :bool =
    let locations = 
        [second;
        {second with Column=second.Column+worldSize.Column};
        {second with Row=second.Row+worldSize.Row};
        {Column=second.Column+worldSize.Column;Row=second.Row+worldSize.Row}]
    locations
    |> Seq.fold (fun failures location->
        if distanceFormulaTest maximum first second then
            failures + 1
        else
            failures) 0
    |> (=) locations.Length




