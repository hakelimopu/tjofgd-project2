open SDLUtility
open SDLGeometry
open System

type GameState =
    {X:int<px>;Y:int<px>}

let renderHandler (renderer:SDLRender.Renderer) (texture:SDLTexture.Texture) (surface:SDLSurface.Surface) (bitmap:SDLSurface.Surface) (state:GameState) :unit =
    let screenSpace = {X=0<px>;Y=0<px>;Width=160<px>;Height=120<px>}
    renderer |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) |> ignore
    renderer |> SDLRender.clear |> ignore

    surface
    |> SDLSurface.upperBlit None bitmap None
    |> ignore

    surface
    |> SDLSurface.fillRect (Some {X=state.X;Y=state.Y;Width=10<px>;Height=10<px>}) 0xFFFF0000u
    |> ignore

    surface
    |> SDLSurface.fillRect (Some {X=10<px>;Y=10<px>;Width=10<px>;Height=10<px>}) 0xFF0000FFu
    |> ignore

    texture
    |> SDLTexture.update None surface
    |> ignore

    renderer |> SDLRender.copy texture screenSpace screenSpace |> ignore
    renderer |> SDLRender.present 

let eventHandler (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit x -> None
    | SDLEvent.KeyDown x -> 
        match x.Keysym.Sym with
        | 0x1B -> None
        | 0x4000004F -> Some {state with X=state.X+5<px>} //right
        | 0x40000050 -> Some {state with X=state.X-5<px>} //left
        | 0x40000051 -> Some {state with Y=state.Y+5<px>} //down
        | 0x40000052 -> Some {state with Y=state.Y-5<px>} //up
        | _ -> Some state
    | _ -> Some state

let rec eventPump (renderHandler:'TState->unit) (eventHandler:SDLEvent.Event->'TState->'TState option) (state:'TState) : unit =
    match SDLEvent.pollEvent() with
    | Some event ->
        match state |> eventHandler event with
        | Some newState -> eventPump renderHandler eventHandler newState
        | None -> ()
    | None -> 
        state
        |> renderHandler
        eventPump renderHandler eventHandler state

[<EntryPoint>]
let main argv = 
    let version = SDLVersion.getVersion()

    (version.Major, version.Minor, version.Patch)
    |||> printfn "SDL Version = %i.%i.%i"

    (SDLVersion.getRevision(),SDLVersion.getRevisionNumber())
    ||> printfn "Revision = '%s'\r\nRevision Number = %i"

    SDL.init(SDL.Init.Video ||| SDL.Init.Events)
    |> printfn "Init Result = %b"

    SDLPixel.RGBA8888Format 
    |> SDLPixel.formatEnumName
    |> printfn "Format Name = %s"

    let mainWindow = SDLWindow.create "test" 100<px> 100<px> 640<px> 480<px> 0u

    let mainRenderer = SDLRender.create mainWindow -1 SDLRender.Flags.Accelerated

    let surface = SDLSurface.createRGB (160<px>,120<px>,32<bit/px>) (0x00FF0000u,0x0000FF00u,0x000000FFu,0xFF000000u)

    let bitmap = SDLSurface.loadBmp SDLPixel.ARGB8888Format "Content/smile.bmp"

    surface |> SDLSurface.fillRect (Some {X=0<px>;Y=0<px>;Width=160<px>;Height=120<px>}) 0xFF000000u |> ignore
    surface |> SDLSurface.fillRect (Some {X=0<px>;Y=0<px>;Width=8<px>;Height=8<px>}) 0xFFFFFFFFu |> ignore
   
    let mainTexture = mainRenderer |> SDLTexture.create SDLPixel.ARGB8888Format SDLTexture.Access.Streaming (160<px>,120<px>)

    mainRenderer |> SDLRender.setLogicalSize (160<px>,120<px>) |> ignore

    eventPump (renderHandler mainRenderer mainTexture surface bitmap) eventHandler {X=0<px>;Y=0<px>}

    surface |> SDLSurface.free

    bitmap |> SDLSurface.free

    mainTexture |> SDLTexture.destroy

    mainRenderer |> SDLRender.destroy 

    mainWindow |> SDLWindow.destroy

    SDL.quit()

    printfn "All done! Press any key."
    Console.ReadKey()
    |> ignore

    0
