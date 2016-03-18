module RenderCell

open CellLocation
open System.Text

//cell colors... maps to colors on a DOS text screen
type RenderCellColor = 
    | Black         =  0
    | Blue          =  1
    | Green         =  2
    | Cyan          =  3
    | Red           =  4
    | Magenta       =  5
    | Brown         =  6
    | White         =  7
    | DarkGray      =  8
    | BrightBlue    =  9
    | BrightGreen   = 10
    | BrightCyan    = 11
    | BrightRed     = 12
    | BrightMagenta = 13
    | BrightYellow  = 14
    | BrightWhite   = 15

//a text cell with a foreground and background color
type RenderCell =
    {Character:byte;
     Foreground:RenderCellColor;
     Background:RenderCellColor}

//write a string character by character, moving one column right after each one
let writeText (location:CellLocation) (foreground:RenderCellColor) (background:RenderCellColor) (text:string) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let bytes = Encoding.ASCII.GetBytes(text)

    let nextPosition (initial:CellLocation) :CellLocation=
        {initial with Column = initial.Column + 1<cell>}

    let writeCell (grid:CellMap<RenderCell>,position:CellLocation) (character:byte) :CellMap<RenderCell> * CellLocation = 
        (grid 
         |> Map.add position {Character=character;Foreground=foreground;Background=background},

         position 
         |> nextPosition)

    ((renderGrid,location), bytes)
    ||> Seq.fold writeCell
    |> fst

//create a sequence of cells length long starting at start
let private cellSequence (start:int<cell>) (length:int<cell>) : seq<int<cell>> =
    let emitter (current:int<cell>,remaining:int<cell>) =
        if remaining=0<cell> then
            None
        else
            Some (current, (current + 1<cell>, remaining - 1<cell>))
    
    (start,length)
    |> Seq.unfold emitter

//clear out a rectangular area on a rendergrid
let clearRectangle (location:CellLocation) (size:CellLocation) (foreground:RenderCellColor) (background:RenderCellColor) (character:byte) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let columnAccumulator rows cell grid column =
        let rowAccumulator cell column grid row = 
            grid 
            |> Map.add {Column=column;Row=row} cell

        (grid, rows)
        ||> Seq.fold (rowAccumulator cell column)

    let renderCell = 
        {Character=character;
         Foreground=foreground;
         Background=background}

    let columnSequence, rowSequence = (location.Column,  size.Column) ||> cellSequence, (location.Row, size.Row) ||> cellSequence

    (renderGrid, columnSequence)
    ||> Seq.fold (columnAccumulator rowSequence renderCell)

//not abandoning! just procrastinating!
//NS = BA
//EW = CD
//NE = C8
//SE = C9
//SW = BB
//NW = BC



