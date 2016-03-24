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
type DistanceFormulaTestFunc = int<cell> -> CellLocation -> CellLocation -> bool

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

[<ExcludeFromCodeCoverage>]
exception NegativeValue

//checking distance
let private distanceFormulaTest (maximum:int<cell>) (first:CellLocation) (second:CellLocation) :bool =
    if maximum < 0<cell> then
        raise NegativeValue
    else
        let deltaX, deltaY = second.Column - first.Column, second.Row - first.Row
        (deltaX * deltaX + deltaY * deltaY) > maximum * maximum

//in a wrapped world, distance determination is based on actual location and three "ghost" locations
let private fourWrappedLocations (worldSize:CellLocation) (location:CellLocation) : CellLocation list = 
    [location;
    {location with Column=location.Column+worldSize.Column};
    {location with Row=location.Row+worldSize.Row};
    {Column=location.Column+worldSize.Column;Row=location.Row+worldSize.Row}]

//distance formula applied to all four versions of a location on a torus
let distanceFormulaTestWrapped (worldSize:CellLocation) (maximum:int<cell>) (first:CellLocation) (second:CellLocation) :bool =
    if (worldSize.Column <= 0<cell>) || (worldSize.Row <= 0<cell>) then
        raise InvalidWorldSize
    else
        let locations = 
            second
            |> fourWrappedLocations worldSize

        let countFailures (accumulator:int) (location:CellLocation) :int =
            if distanceFormulaTest maximum first second then
                accumulator + 1
            else
                accumulator

        (0, locations)
        ||> Seq.fold countFailures
        |> (=) locations.Length




