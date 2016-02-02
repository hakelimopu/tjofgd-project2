module SDLRect

#nowarn "9"

open System.Runtime.InteropServices
open System
open SDLUtility

[<StructLayout(LayoutKind.Sequential)>]
type private SDL_Point =
    struct
        val x: int
        val y: int
    end

[<StructLayout(LayoutKind.Sequential)>]
type private SDL_Rect = 
    struct
        val x :int
        val y :int
        val w :int
        val h :int
    end

type Point = {X: int<px>; Y: int<px>}

type Rectangle = {X: int<px>; Y: int<px>; Width: int<px>; Height: int<px>}

let pointInRect (point: Point) (rectangle:Rectangle) :bool =
    point.X >= rectangle.X && point.Y >=rectangle.Y && point.X < (rectangle.X + rectangle.Width) && point.Y < (rectangle.Y + rectangle.Height)

let isEmpty (rectangle:Rectangle option) :bool =
    match rectangle with
    | None -> true
    | Some r -> r.Width <= 0<px> || r.Height <=0<px>


let equals (first:Rectangle option) (second:Rectangle option) : bool =
    match (first,second) with
    | (None, None) -> true
    | (Some r1, Some r2) -> r1.X=r2.X && r1.Y=r2.Y && r1.Width = r2.Width && r1.Height = r2.Height
    | _ -> false