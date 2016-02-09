module Render

open GameState
open SDLUtility
open SDLGeometry

type RenderingContext =
    {Renderer:SDLRender.Renderer;
    Texture:SDLTexture.Texture;
    Surface:SDLSurface.Surface}

let draw (context:RenderingContext) (state:GameState) :unit =
    context.Renderer |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) |> ignore
    context.Renderer |> SDLRender.clear |> ignore

    context.Surface
    |> SDLSurface.fillRect None {Red=0uy;Green=0uy;Blue=0uy;Alpha=255uy}
    |> ignore

    context.Surface
    |> SDLSurface.fillRect (Some {X=state.X;Y=state.Y;Width=10<px>;Height=10<px>}) {Red=255uy;Green=0uy;Blue=0uy;Alpha=255uy}
    |> ignore

    context.Surface
    |> SDLSurface.fillRect (Some {X=10<px>;Y=10<px>;Width=10<px>;Height=10<px>}) {Red=0uy;Green=0uy;Blue=255uy;Alpha=255uy}
    |> ignore

    context.Texture
    |> SDLTexture.update None context.Surface
    |> ignore

    context.Renderer |> SDLRender.copy context.Texture None None |> ignore
    context.Renderer |> SDLRender.present 
