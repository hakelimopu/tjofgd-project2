module CellLocation

open SDLUtility

[<Measure>] type cell

type CellLocation =
    {Column:int<cell>;Row:int<cell>}

let sumLocations (first:CellLocation) (second:CellLocation) :CellLocation =
    {Column=first.Column+second.Column;Row=first.Row+second.Row}

let pixelsPerColumn = 8<px/cell>
let pixelsPerRow = 8<px/cell>

type CellMap<'T> = Map<CellLocation,'T>




