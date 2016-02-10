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
    Sprites:Map<int,Sprite>;
    WorkSurface:SDLSurface.Surface;
    Random:System.Random}

let draw (context:RenderingContext) (state:GameState) :unit =
    context.Renderer |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) |> ignore
    context.Renderer |> SDLRender.clear |> ignore

    context.Surface
    |> SDLSurface.fillRect None {Red=255uy;Green=0uy;Blue=255uy;Alpha=255uy}
    |> ignore

    for x in [0..39] do
        for y in [0..29] do
            let foreground = {Red=context.Random.Next(256) |> byte;Green=context.Random.Next(256) |> byte;Blue=context.Random.Next(256) |> byte;Alpha=0uy}
            let background = {Red=context.Random.Next(256) |> byte;Green=context.Random.Next(256) |> byte;Blue=context.Random.Next(256) |> byte;Alpha=0uy}
            let sprite = context.Sprites.[context.Random.Next(256)]

            context.WorkSurface
            |> SDLSurface.fillRect None foreground
            |> ignore

            sprite
            |> blitSprite {X=0<px>;Y=0<px>} context.WorkSurface
            |> ignore

            context.Surface
            |> SDLSurface.fillRect (Some {X=x*8<px>;Y=y*8<px>;Width=8<px>;Height=8<px>}) background
            |> ignore

            //TODO: this ain't color keyin!
            context.Surface
            |> SDLSurface.blit (Some {X=0<px>;Y=0<px>;Width=8<px>;Height=8<px>}) context.WorkSurface (Some {X=x*8<px>;Y=y*8<px>;Width=8<px>;Height=8<px>})
            |> ignore

    context.Texture
    |> SDLTexture.update None context.Surface
    |> ignore

    context.Renderer |> SDLRender.copy context.Texture None None |> ignore
    context.Renderer |> SDLRender.present 
