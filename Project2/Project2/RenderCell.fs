module RenderCell

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



