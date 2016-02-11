module GameState

open SDLUtility

[<Measure>] type cell

type CellLocation =
    {Column:int<cell>;Row:int<cell>}

let pixelsPerColumn = 8<px/cell>
let pixelsPerRow = 8<px/cell>

type CellColor = 
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

type Cell =
    {Character:byte;Foreground:CellColor;Background:CellColor}

type CellMap = Map<CellLocation,Cell>

type PlayState =
    {Grid:CellMap}

type GameState = 
    | PlayState of PlayState



