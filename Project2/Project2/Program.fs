﻿open SDLUtility
open SDLGeometry
open System

open GameState
open SDLPixel

[<EntryPoint>]
let main argv = 

    use mainWindow = SDLWindow.create "test" 100<px> 100<px> 640<px> 480<px> 0u

    let mainRenderer = SDLRender.create mainWindow.Pointer -1 SDLRender.Flags.Accelerated

    let surface = SDLSurface.createRGB (160<px>,120<px>,32<bit/px>) (0x00FF0000u,0x0000FF00u,0x000000FFu,0xFF000000u)

    let mainTexture = mainRenderer |> SDLTexture.create SDLPixel.ARGB8888Format SDLTexture.Access.Streaming (160<px>,120<px>)

    mainRenderer |> SDLRender.setLogicalSize (160<px>,120<px>) |> ignore

    EventPump.eventPump (Render.draw {Renderer=mainRenderer;Texture=mainTexture;Surface=surface}) EventHandler.handleEvent {X=0<px>;Y=0<px>}

    surface |> SDLSurface.free

    mainTexture |> SDLTexture.destroy

    mainRenderer |> SDLRender.destroy 

    SDL.quit()

    0
