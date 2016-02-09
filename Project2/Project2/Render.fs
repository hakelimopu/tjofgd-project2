module Render

open GameState
open SDLUtility
open SDLGeometry

type Sprite =
    {Surface:SDLSurface.Surface;
    Bounds:SDLGeometry.Rectangle option}

let createSprite surface bounds =
    {Surface=surface;Bounds=bounds}

type RenderingContext =
    {Renderer:SDLRender.Renderer;
    Texture:SDLTexture.Texture;
    Surface:SDLSurface.Surface;
    Sprites:Map<int,Sprite>;
    Random:System.Random}

let draw (context:RenderingContext) (state:GameState) :unit =
    context.Renderer |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) |> ignore
    context.Renderer |> SDLRender.clear |> ignore

    context.Surface
    |> SDLSurface.fillRect None {Red=0uy;Green=0uy;Blue=0uy;Alpha=255uy}
    |> ignore

    for x in [0..39] do
        for y in [0..29] do
            let sprite = context.Sprites.[context.Random.Next(256)]
            context.Surface
            |> SDLSurface.blit sprite.Bounds sprite.Surface  (Some {X=x * 8<px>;Y=y * 8<px>;Width=8<px>;Height=8<px>})
            |> ignore

    context.Texture
    |> SDLTexture.update None context.Surface
    |> ignore

    context.Renderer |> SDLRender.copy context.Texture None None |> ignore
    context.Renderer |> SDLRender.present 
