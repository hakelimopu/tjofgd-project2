module CellLocation

open System.Diagnostics.CodeAnalysis

//in a grid, we measure things in cells
[<Measure>] type cell

//a grid location
[<ExcludeFromCodeCoverage>]
type CellLocation =
    {Column: int<cell>;
     Row   : int<cell>}

//the grid itself is a simple map
type CellMap<'T> = Map<CellLocation,'T>

let emptyCellMap<'T> :CellMap<'T> = Map.empty<CellLocation,'T>

//more or less, function interfaces
type SumLocationsFunc = CellLocation -> CellLocation -> CellLocation
type CheckLocationsFunc = CellLocation -> CellLocation -> bool
type VectorToLocationFunc = CellLocation -> CellLocation -> float<cell> * float

[<ExcludeFromCodeCoverage>]
exception InvalidWorldSize

//making a toroid world
let wrapLocation (worldSize:CellLocation) (location:CellLocation) = 
    if (worldSize.Column <= 0<cell>) || (worldSize.Row <= 0<cell>) then
        raise InvalidWorldSize
    else
        let wrapDimension (dimensionSize:int<cell>) (dimensionValue:int<cell>) :int<cell>=
            let newDimensionValue = 
                dimensionValue % dimensionSize

            if newDimensionValue < 0<cell> then 
                newDimensionValue + dimensionSize 
            else 
                newDimensionValue

        {Column = (worldSize.Column, location.Column) ||> wrapDimension; 
         Row    = (worldSize.Row   , location.Row   ) ||> wrapDimension}

//must be a way to walk from one location to another!
let private sumLocations (first:CellLocation) (second:CellLocation) :CellLocation =
    {Column = first.Column + second.Column;
     Row    = first.Row    + second.Row}

//adding locations in a toroid world
let sumLocationsWrapped (worldSize:CellLocation) (first:CellLocation) (second:CellLocation) :CellLocation =
    (first,second)
    ||> sumLocations
    |> wrapLocation worldSize

let private vectorMagnitude (vector:CellLocation) :float<cell> =
    ((vector.Column * vector.Column + vector.Row * vector.Row)
    / 1<cell^2>
    |> float
    |> sqrt)
    * 1.0<cell>

let private vectorDirection (vector:CellLocation) :float =
    atan2 (vector.Row / 1<cell> |> float) (vector.Column / 1<cell> |> float)

let private vectorToLocation (start:CellLocation) (finish:CellLocation) :float<cell> * float =
    let delta = 
        {Column = finish.Column - start.Column;
         Row    = finish.Row    - start.Row}
    let magnitude = delta |> vectorMagnitude
    (magnitude, delta |> vectorDirection)

[<ExcludeFromCodeCoverage>]
exception NegativeValue

//checking distance
let private distanceFormulaTest (maximum:int<cell>) (first:CellLocation) (second:CellLocation) :bool =
    if maximum < 0<cell> then
        raise NegativeValue
    else
        let deltaX, deltaY = second.Column - first.Column, second.Row - first.Row
        (deltaX * deltaX + deltaY * deltaY) > maximum * maximum

let private nineWrappedLocations (worldSize:CellLocation) (location:CellLocation) : CellLocation list = 
    [location;
    {location with Column=location.Column+worldSize.Column};
    {location with Column=location.Column-worldSize.Column};
    {location with Row=location.Row+worldSize.Row};
    {location with Row=location.Row-worldSize.Row};
    {Column=location.Column+worldSize.Column;Row=location.Row+worldSize.Row};
    {Column=location.Column+worldSize.Column;Row=location.Row-worldSize.Row};
    {Column=location.Column-worldSize.Column;Row=location.Row+worldSize.Row};
    {Column=location.Column-worldSize.Column;Row=location.Row-worldSize.Row}]

//distance formula applied to all four versions of a location on a torus
let distanceFormulaTestWrapped (worldSize:CellLocation) (maximum:int<cell>) (first:CellLocation) (second:CellLocation) :bool =
    if (worldSize.Column <= 0<cell>) || (worldSize.Row <= 0<cell>) then
        raise InvalidWorldSize
    else
        let locations = 
            second
            |> nineWrappedLocations worldSize

        let countFailures (accumulator:int) (location:CellLocation) :int =
            if distanceFormulaTest maximum first second then
                accumulator + 1
            else
                accumulator

        (0, locations)
        ||> Seq.fold countFailures
        |> (=) locations.Length

let vectorToLocationWrapped (worldSize:CellLocation) (start:CellLocation) (finish:CellLocation) :float<cell> * float =
    finish
    |> nineWrappedLocations worldSize
    |> List.map (fun e -> vectorToLocation start e)
    |> List.minBy (fun (m,d) -> m)

let make (column:int<cell>) (row:int<cell>) :CellLocation =
    {Column = column; Row = row}

let private generateRadiusCellLocationEmitter (radius:int<cell>) (column:int<cell>) (current:int<cell>, maximum:int<cell>) : (Set<CellLocation> * (int<cell> * int<cell>)) option =
    if current > maximum then
        None
    else
        let location =
            if column * column + current * current <= radius * radius then
                [(column,current) ||> make] |> Set.ofSeq
            else
                Set.empty
        Some (location, (current + 1<cell>, maximum))

let private generateRadiusColumnEmitter (radius:int<cell>) (rows:int<cell> * int<cell>) (current:int<cell>, maximum:int<cell>) : (Set<CellLocation> * (int<cell> * int<cell>)) option =
    if current > maximum then
        None
    else
        let locations =
            rows
            |> Seq.unfold (generateRadiusCellLocationEmitter radius current)
            |> Seq.reduce Set.union

        Some (locations, (current + 1<cell>, maximum))

let generateRadius (radius:int<cell>) :Set<CellLocation> =
    (-radius, radius)
    |> Seq.unfold (generateRadiusColumnEmitter radius (-radius, radius))
    |> Seq.reduce Set.union
