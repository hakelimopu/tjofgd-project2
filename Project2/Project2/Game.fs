module Game

open SDL
open SDL.Geometry
open SDL.Pixel
open GameState
open CellLocation
open RenderCell
open MapCell
open MapCreate
open Render
open Random
open IdleHandler
open GridRenderer

let runGame (randomFunc:RandomFunc) = 
    use system = new SDL.Init.System(SDL.Init.Init.Video ||| SDL.Init.Init.Events)

    use mainWindow = SDL.Window.create "Splorr!! Island Interloper" (100<px>,100<px>) (640<px>,480<px>) 0u

    use mainRenderer = SDL.Render.create mainWindow -1 SDL.Render.Flags.Accelerated

    use surface = SDL.Surface.createRGB (320<px>,240<px>,32<bit/px>) (0x00FF0000u,0x0000FF00u,0x000000FFu,0x00000000u)

    use workSurface = SDL.Surface.createRGB (8<px>,8<px>,32<bit/px>) (0x00FF0000u,0x0000FF00u,0x000000FFu,0x00000000u)

    workSurface
    |> SDL.Surface.setColorKey (Some {Red=0uy;Green=0uy;Blue=0uy;Alpha=0uy})
    |> ignore

    use bitmap = SDL.Surface.loadBmp SDL.Pixel.RGB888Format "Content/romfont8x8.bmp"

    bitmap
    |> SDL.Surface.setColorKey (Some {Red=255uy;Green=255uy;Blue=255uy;Alpha=0uy})
    |> ignore

    use mainTexture = mainRenderer |> SDL.Texture.create SDL.Pixel.RGB888Format SDL.Texture.Access.Streaming (320<px>,240<px>)

    mainRenderer |> SDL.Render.setLogicalSize (320<px>,240<px>) |> ignore

    //graphics setup

    let sprites = 
        [0uy..255uy]
        |> Seq.map(fun index-> (index, (Render.createSprite bitmap {X=8<px>*((index |> int) % 16); Y=8<px>*((index |> int) / 16);Width=8<px>;Height=8<px>})))
        |> Map.ofSeq
    
    //game setup
    let sumLocationsFunc = sumLocationsWrapped Constants.WorldSize
    let setVisibleFunc = setVisibleWrapped  Constants.WorldSize
    let distanceFormulaTestFunc = distanceFormulaTestWrapped Constants.WorldSize
    let setTerrainFunc = setTerrainWrapped Constants.WorldSize
    let setObjectFunc = setObjectWrapped Constants.WorldSize
    let vectorToLocationFunc = vectorToLocationWrapped Constants.WorldSize

    let createFunc () :GameState<CellMap<RenderCell>>= 
        let initialActors, initialMap =
            createWorld sumLocationsFunc (distanceFormulaTestFunc Constants.IslandDistance ) setVisibleFunc setTerrainFunc setObjectFunc Constants.WorldSize randomFunc
        let state: PlayState<CellMap<RenderCell>> = {RenderData = CellLocation.emptyCellMap;MapGrid=initialMap;Encounters=None;Actors=initialActors}
        {state with MapGrid = state |> updateVisibleFlags sumLocationsFunc setVisibleFunc} |> PlayState

    //rendering setup
    let renderingContext:RenderingContext = {Renderer=mainRenderer;Texture=mainTexture;Surface=surface;Sprites = sprites;WorkSurface=workSurface}
    let renderFunc = Render.draw renderingContext

    //event handler setup
    let eventHandler = EventHandler.handleEvent<CellMap<RenderCell>> (gridRenderer vectorToLocationFunc) sumLocationsFunc setVisibleFunc createFunc Constants.WorldSize randomFunc

    EventPump.eventPump 
        SDL.Event.pollEvent
        renderFunc 
        eventHandler 
        (createFunc())
