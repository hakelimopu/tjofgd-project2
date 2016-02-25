module RenderCell

open CellLocation
open System.Text

type RenderCellColor = 
    | Black = 0
    | Blue = 1
    | Green = 2
    | Cyan = 3
    | Red = 4
    | Magenta = 5
    | Brown = 6
    | White = 7
    | DarkGray = 8
    | BrightBlue = 9
    | BrightGreen = 10
    | BrightCyan = 11
    | BrightRed = 12
    | BrightMagenta = 13
    | BrightYellow = 14
    | BrightWhite = 15

type RenderCell =
    {Character:byte;Foreground:RenderCellColor;Background:RenderCellColor}

let internal writeText (location:CellLocation) (foreground:RenderCellColor) (background:RenderCellColor) (text:string) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let bytes = Encoding.ASCII.GetBytes(text)
    ((renderGrid,location),bytes)
    ||> Seq.fold(fun (grid,position) character->
        (grid |> Map.add position {Character=character;Foreground=foreground;Background=background},{position with Column=position.Column+1<cell>}))
    |> fst

let cellSequence (start:int<cell>,length:int<cell>) : seq<int<cell>> =
    [(start/1<cell>)..((start/1<cell>)+(length/1<cell>)-1)]
    |> Seq.map(fun v -> v * 1<cell>)

let internal clearRectangle (location:CellLocation) (size:CellLocation) (foreground:RenderCellColor) (background:RenderCellColor) (character:byte) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let renderCell = {Character=character;Foreground=foreground;Background=background}
    (renderGrid,cellSequence (location.Column, size.Column))
    ||> Seq.fold(fun outerGrid column ->
        (outerGrid, cellSequence (location.Row, size.Row))
        ||> Seq.fold(fun innerGrid row -> innerGrid |> Map.add {Column=column;Row=row} renderCell))

//not abandoning! just procrastinating!
//NS = BA
//EW = CD
//NE = C8
//SE = C9
//SW = BB
//NW = BC



