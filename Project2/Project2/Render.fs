module Render

open GameState
open SDL
open SDL.Geometry
open SDL.Pixel
open CellLocation
open RenderCell


type Sprite =
    {Surface: SDL.Surface.Surface;
     Bounds:  Rectangle}

let createSprite (surface:SDL.Surface.Surface) (bounds:Rectangle) :Sprite =
    {Surface=surface;
     Bounds=bounds}

let blitSprite (dst:Point) (dstSurface:SDL.Surface.Surface) (sprite:Sprite) :bool=
    dstSurface
    |> SDL.Surface.blit (Some sprite.Bounds) sprite.Surface (Some {X=dst.X;Y=dst.Y;Width=sprite.Bounds.Width;Height=sprite.Bounds.Height})

type RenderingContext =
    {Renderer:    SDL.Render.Renderer;
     Texture:     SDL.Texture.Texture;
     Surface:     SDL.Surface.Surface;
     Sprites:     Map<byte,Sprite>;
     WorkSurface: SDL.Surface.Surface}


let draw (context:RenderingContext) (state:GameState<_>) :unit =
    let pixelsPerColumn = 8<px/cell>
    let pixelsPerRow = 8<px/cell>
    let palette = 
        [(RenderCellColor.Black        , {Red = 0x01uy; Green = 0x01uy; Blue = 0x01uy; Alpha = 0x00uy});
         (RenderCellColor.Blue         , {Red = 0x00uy; Green = 0x00uy; Blue = 0xAAuy; Alpha = 0x00uy});
         (RenderCellColor.Green        , {Red = 0x00uy; Green = 0xAAuy; Blue = 0x00uy; Alpha = 0x00uy});
         (RenderCellColor.Cyan         , {Red = 0x00uy; Green = 0xAAuy; Blue = 0xAAuy; Alpha = 0x00uy});
         (RenderCellColor.Red          , {Red = 0xAAuy; Green = 0x00uy; Blue = 0x00uy; Alpha = 0x00uy});
         (RenderCellColor.Magenta      , {Red = 0xAAuy; Green = 0x00uy; Blue = 0xAAuy; Alpha = 0x00uy});
         (RenderCellColor.Brown        , {Red = 0xAAuy; Green = 0x55uy; Blue = 0x00uy; Alpha = 0x00uy});
         (RenderCellColor.White        , {Red = 0xAAuy; Green = 0xAAuy; Blue = 0xAAuy; Alpha = 0x00uy});
         (RenderCellColor.DarkGray     , {Red = 0x55uy; Green = 0x55uy; Blue = 0x55uy; Alpha = 0x00uy});
         (RenderCellColor.BrightBlue   , {Red = 0x55uy; Green = 0x55uy; Blue = 0xFFuy; Alpha = 0x00uy});
         (RenderCellColor.BrightGreen  , {Red = 0x55uy; Green = 0xFFuy; Blue = 0x55uy; Alpha = 0x00uy});
         (RenderCellColor.BrightCyan   , {Red = 0x55uy; Green = 0xFFuy; Blue = 0xFFuy; Alpha = 0x00uy});
         (RenderCellColor.BrightRed    , {Red = 0xFFuy; Green = 0x55uy; Blue = 0x55uy; Alpha = 0x00uy});
         (RenderCellColor.BrightMagenta, {Red = 0xFFuy; Green = 0x55uy; Blue = 0xFFuy; Alpha = 0x00uy});
         (RenderCellColor.BrightYellow , {Red = 0xFFuy; Green = 0xFFuy; Blue = 0x55uy; Alpha = 0x00uy});
         (RenderCellColor.BrightWhite  , {Red = 0xFFuy; Green = 0xFFuy; Blue = 0xFFuy; Alpha = 0x00uy})]
        |> Map.ofSeq

    context.Renderer 
    |> SDL.Render.setDrawColor (255uy,0uy,255uy,255uy) 
    |> ignore

    context.Renderer 
    |> SDL.Render.clear 
    |> ignore

    context.Surface
    |> SDL.Surface.fillRect None {Red=255uy;Green=0uy;Blue=255uy;Alpha=255uy}
    |> ignore

    let renderCell (location:CellLocation) (cell:RenderCell) :unit =
        let foreground = palette.[cell.Foreground]
        let background = palette.[cell.Background]
        let sprite = context.Sprites.[cell.Character]

        context.WorkSurface
        |>* SDL.Surface.fillRect None foreground

        sprite
        |>* blitSprite {X=0<px>;Y=0<px>} context.WorkSurface

        let dstRect = 
            {X      = location.Column * pixelsPerColumn;
             Y      = location.Row    * pixelsPerRow;
             Width  = 1<cell>         * pixelsPerColumn;
             Height = 1<cell>         * pixelsPerRow} 
            |> Some

        context.Surface
        |>* SDL.Surface.fillRect dstRect background

        context.Surface
        |>* SDL.Surface.blit (Some {X=0<px>;Y=0<px>;Width=8<px>;Height=8<px>}) context.WorkSurface dstRect

    let renderPlayState (state:PlayState<CellMap<RenderCell>>) = 
        state.RenderData
        |> Map.iter renderCell

    match state with
    | DeadState state  ->
        state |> renderPlayState
    | PlayState state ->
        state |> renderPlayState

    context.Texture
    |>* SDL.Texture.update None context.Surface

    context.Renderer 
    |>* SDL.Render.copy context.Texture None None 

    context.Renderer 
    |> SDL.Render.present 
