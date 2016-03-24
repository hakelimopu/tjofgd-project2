module Render

open GameState
open SDLUtility
open SDLGeometry
open SDLPixel
open CellLocation
open RenderCell


type Sprite =
    {Surface: SDLSurface.Surface;
     Bounds:  SDLGeometry.Rectangle}

let createSprite (surface:SDLSurface.Surface) (bounds:SDLGeometry.Rectangle) :Sprite =
    {Surface=surface;
     Bounds=bounds}

let blitSprite (dst:SDLGeometry.Point) (dstSurface:SDLSurface.Surface) (sprite:Sprite) :bool=
    dstSurface
    |> SDLSurface.blit (Some sprite.Bounds) sprite.Surface (Some {X=dst.X;Y=dst.Y;Width=sprite.Bounds.Width;Height=sprite.Bounds.Height})

type RenderingContext =
    {Renderer:    SDLRender.Renderer;
     Texture:     SDLTexture.Texture;
     Surface:     SDLSurface.Surface;
     Sprites:     Map<byte,Sprite>;
     WorkSurface: SDLSurface.Surface}


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
    |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) 
    |> ignore

    context.Renderer 
    |> SDLRender.clear 
    |> ignore

    context.Surface
    |> SDLSurface.fillRect None {Red=255uy;Green=0uy;Blue=255uy;Alpha=255uy}
    |> ignore

    let renderCell (location:CellLocation) (cell:RenderCell) :unit =
        let foreground = palette.[cell.Foreground]
        let background = palette.[cell.Background]
        let sprite = context.Sprites.[cell.Character]

        context.WorkSurface
        |> SDLSurface.fillRect None foreground
        |> ignore

        sprite
        |> blitSprite {X=0<px>;Y=0<px>} context.WorkSurface
        |> ignore

        let dstRect = 
            {X      = location.Column * pixelsPerColumn;
             Y      = location.Row    * pixelsPerRow;
             Width  = 1<cell>         * pixelsPerColumn;
             Height = 1<cell>         * pixelsPerRow} 
            |> Some

        context.Surface
        |> SDLSurface.fillRect dstRect background
        |> ignore

        context.Surface
        |> SDLSurface.blit (Some {X=0<px>;Y=0<px>;Width=8<px>;Height=8<px>}) context.WorkSurface dstRect
        |> ignore

    let renderPlayState (state:PlayState<CellMap<RenderCell>>) = 
        state.RenderData
        |> Map.iter renderCell

    match state with
    | DeadState state  ->
        state |> renderPlayState
    | PlayState state ->
        state |> renderPlayState

    context.Texture
    |> SDLTexture.update None context.Surface
    |> ignore

    context.Renderer 
    |> SDLRender.copy context.Texture None None 
    |> ignore

    context.Renderer 
    |> SDLRender.present 
