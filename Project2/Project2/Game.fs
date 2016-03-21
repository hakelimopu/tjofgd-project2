module Game

open SDLUtility
open SDLGeometry
open SDLPixel
open GameState
open CellLocation
open RenderCell
open MapCell
open MapCreate
open Render


let runGame() = 
    //SDL setup
    
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

    //graphics setup

    let sprites = 
        [0uy..255uy]
        |> Seq.map(fun index-> (index, (Render.createSprite bitmap {X=8<px>*((index |> int) % 16);Y=8<px>*((index |> int) / 16);Width=8<px>;Height=8<px>})))
        |> Map.ofSeq
    
    //game setup
    let sumLocationsFunc = sumLocationsWrapped Constants.WorldSize
    let setVisibleFunc = setVisibleWrapped  Constants.WorldSize
    let distanceFormulaTestFunc = distanceFormulaTestWrapped Constants.WorldSize
    let setTerrainFunc = setTerrainWrapped Constants.WorldSize
    let setObjectFunc = setObjectWrapped Constants.WorldSize

    let random = new System.Random()

    let createFunc ()= 
        let initialActors, initialMap =
            createWorld sumLocationsFunc distanceFormulaTestFunc setVisibleFunc setTerrainFunc setObjectFunc Constants.WorldSize random
        let state = {PlayState.RenderGrid = Map.empty<CellLocation,RenderCell>;MapGrid=initialMap;Encounters=None;Actors=initialActors}
        {state with MapGrid = state |> updateVisibleFlags sumLocationsFunc setVisibleFunc} |> PlayState

    //rendering setup
    let renderingContext:RenderingContext = {Renderer=mainRenderer;Texture=mainTexture;Surface=surface;Sprites = sprites;WorkSurface=workSurface}
    let renderFunc = Render.draw renderingContext

    //event handler setup
    let eventHandler = EventHandler.handleEvent sumLocationsFunc setVisibleFunc createFunc Constants.WorldSize random

    EventPump.eventPump 
        SDLEvent.pollEvent
        renderFunc 
        eventHandler 
        (createFunc())
