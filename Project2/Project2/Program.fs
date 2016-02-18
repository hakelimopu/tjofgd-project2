open SDLUtility
open SDLGeometry
open SDLPixel
open GameState
open CellLocation
open RenderCell
open MapCell
open MapCreate


[<EntryPoint>]
let main argv = 
    use system = new SDL.System(SDL.Init.Video ||| SDL.Init.Events)

    use mainWindow = SDLWindow.create "test" 100<px> 100<px> 640<px> 480<px> 0u

    use mainRenderer = SDLRender.create mainWindow -1 SDLRender.Flags.Accelerated

    use surface = SDLSurface.createRGB (320<px>,240<px>,32<bit/px>) (0x00FF0000u,0x0000FF00u,0x000000FFu,0x00000000u)

    use workSurface = SDLSurface.createRGB (8<px>,8<px>,32<bit/px>) (0x00FF0000u,0x0000FF00u,0x000000FFu,0x00000000u)

    workSurface
    |> SDLSurface.setColorKey (Some {Red=0uy;Green=0uy;Blue=0uy;Alpha=0uy})
    |> ignore

    use bitmap = SDLSurface.loadBmp SDLPixel.RGB888Format "Content/romfont8x8.bmp"

    bitmap
    |> SDLSurface.setColorKey (Some {Red=255uy;Green=255uy;Blue=255uy;Alpha=0uy})
    |> ignore

    use mainTexture = mainRenderer |> SDLTexture.create SDLPixel.RGB888Format SDLTexture.Access.Streaming (320<px>,240<px>)

    mainRenderer |> SDLRender.setLogicalSize (320<px>,240<px>) |> ignore

    let sprites = 
        [0uy..255uy]
        |> Seq.map(fun index-> (index, (Render.createSprite bitmap {X=8<px>*((index |> int) % 16);Y=8<px>*((index |> int) / 16);Width=8<px>;Height=8<px>})))
        |> Map.ofSeq
    
    let random = new System.Random()

    let initialMap =
        createWorld (sumLocationsWrapped Constants.WorldSize) (distanceFormulaTestWrapped Constants.WorldSize) (setVisibleWrapped Constants.WorldSize) (setTerrainWrapped Constants.WorldSize) (setObjectWrapped Constants.WorldSize) random

    EventPump.eventPump (Render.draw {Renderer=mainRenderer;Texture=mainTexture;Surface=surface;Sprites = sprites;WorkSurface=workSurface}) (EventHandler.handleEvent (sumLocationsWrapped Constants.WorldSize) (setVisibleWrapped  Constants.WorldSize)) ({PlayState.RenderGrid = Map.empty<CellLocation,RenderCell>;MapGrid=initialMap} |> PlayState)

    0
