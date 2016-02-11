module Render

open GameState
open SDLUtility
open SDLGeometry
open SDLPixel

type Sprite =
    {Surface:SDLSurface.Surface;
    Bounds:SDLGeometry.Rectangle}

let createSprite surface bounds =
    {Surface=surface;Bounds=bounds}

let blitSprite (dst:SDLGeometry.Point) (dstSurface:SDLSurface.Surface) (sprite:Sprite) :bool=
    dstSurface
    |> SDLSurface.blit (Some sprite.Bounds) sprite.Surface (Some {X=dst.X;Y=dst.Y;Width=sprite.Bounds.Width;Height=sprite.Bounds.Height})

type RenderingContext =
    {Renderer:SDLRender.Renderer;
    Texture:SDLTexture.Texture;
    Surface:SDLSurface.Surface;
    Sprites:Map<byte,Sprite>;
    WorkSurface:SDLSurface.Surface}

let private palette = 
    [(CellColor.Black        ,{Red=0x01uy;Green=0x01uy;Blue=0x01uy;Alpha=0x00uy});
    (CellColor.Blue         ,{Red=0x00uy;Green=0x00uy;Blue=0xAAuy;Alpha=0x00uy});
    (CellColor.Green        ,{Red=0x00uy;Green=0xAAuy;Blue=0x00uy;Alpha=0x00uy});
    (CellColor.Cyan         ,{Red=0x00uy;Green=0xAAuy;Blue=0xAAuy;Alpha=0x00uy});
    (CellColor.Red          ,{Red=0xAAuy;Green=0x00uy;Blue=0x00uy;Alpha=0x00uy});
    (CellColor.Magenta      ,{Red=0xAAuy;Green=0x00uy;Blue=0xAAuy;Alpha=0x00uy});
    (CellColor.Brown        ,{Red=0xAAuy;Green=0x55uy;Blue=0x00uy;Alpha=0x00uy});
    (CellColor.White        ,{Red=0xAAuy;Green=0xAAuy;Blue=0xAAuy;Alpha=0x00uy});
    (CellColor.DarkGray     ,{Red=0x55uy;Green=0x55uy;Blue=0x55uy;Alpha=0x00uy});
    (CellColor.BrightBlue   ,{Red=0x55uy;Green=0x55uy;Blue=0xFFuy;Alpha=0x00uy});
    (CellColor.BrightGreen  ,{Red=0x55uy;Green=0xFFuy;Blue=0x55uy;Alpha=0x00uy});
    (CellColor.BrightCyan   ,{Red=0x55uy;Green=0xFFuy;Blue=0xFFuy;Alpha=0x00uy});
    (CellColor.BrightRed    ,{Red=0xFFuy;Green=0x55uy;Blue=0x55uy;Alpha=0x00uy});
    (CellColor.BrightMagenta,{Red=0xFFuy;Green=0x55uy;Blue=0xFFuy;Alpha=0x00uy});
    (CellColor.BrightYellow ,{Red=0xFFuy;Green=0xFFuy;Blue=0x55uy;Alpha=0x00uy});
    (CellColor.BrightWhite  ,{Red=0xFFuy;Green=0xFFuy;Blue=0xFFuy;Alpha=0x00uy})]
    |> Map.ofSeq

let draw (context:RenderingContext) (state:GameState) :unit =
    context.Renderer |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) |> ignore
    context.Renderer |> SDLRender.clear |> ignore

    context.Surface
    |> SDLSurface.fillRect None {Red=255uy;Green=0uy;Blue=255uy;Alpha=255uy}
    |> ignore

    match state with
    | PlayState state ->
        state.Grid
        |> Map.iter(fun location cell -> 
            let foreground = palette.[cell.Foreground]
            let background = palette.[cell.Background]
            let sprite = context.Sprites.[cell.Character]

            context.WorkSurface
            |> SDLSurface.fillRect None foreground
            |> ignore

            sprite
            |> blitSprite {X=0<px>;Y=0<px>} context.WorkSurface
            |> ignore

            let dstRect = (Some {X=location.Column * pixelsPerColumn;Y=location.Row * pixelsPerRow;Width=1<cell> * pixelsPerColumn;Height=1<cell> * pixelsPerRow})

            context.Surface
            |> SDLSurface.fillRect dstRect background
            |> ignore

            context.Surface
            |> SDLSurface.blit (Some {X=0<px>;Y=0<px>;Width=8<px>;Height=8<px>}) context.WorkSurface dstRect
            |> ignore
            )
        


    context.Texture
    |> SDLTexture.update None context.Surface
    |> ignore

    context.Renderer |> SDLRender.copy context.Texture None None |> ignore
    context.Renderer |> SDLRender.present 
