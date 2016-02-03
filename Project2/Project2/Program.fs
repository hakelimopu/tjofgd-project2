open SDLUtility
open SDLGeometry
open System

let rec eventPump window renderer texture =
    let screenSpace = {X=0<px>;Y=0<px>;Width=80<px>;Height=120<px>}

    renderer |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) |> ignore
    renderer |> SDLRender.clear |> ignore
    renderer |> SDLRender.copy texture screenSpace screenSpace |> ignore
    renderer |> SDLRender.present 

    match SDLEvent.waitEvent() with
    | Some (SDLEvent.MouseButtonUp x) -> ()
    | Some _ -> eventPump window renderer texture
    | None -> ()

[<EntryPoint>]
let main argv = 
    let version = SDLVersion.getVersion()

    (version.Major, version.Minor, version.Patch)
    |||> printfn "SDL Version = %i.%i.%i"

    (SDLVersion.getRevision(),SDLVersion.getRevisionNumber())
    ||> printfn "Revision = '%s'\r\nRevision Number = %i"

    SDL.init(SDL.Init.Video ||| SDL.Init.Events)
    |> printfn "Init Result = %b"

    let mainWindow = SDLWindow.create "test" 100<px> 100<px> 640<px> 480<px> 0u

    let mainRenderer = SDLRender.create mainWindow -1 SDLRender.SDL_RENDERER_ACCELERATED

    let mainTexture = mainRenderer |> SDLTexture.create SDLPixel.ARGB8888 SDLTexture.SDL_TEXTUREACCESS_STREAMING (160<px>,120<px>)

    mainRenderer |> SDLRender.setLogicalSize (160<px>,120<px>) |> ignore

    eventPump mainWindow mainRenderer mainTexture

    mainTexture |> SDLTexture.destroy

    mainRenderer |> SDLRender.destroy 

    mainWindow |> SDLWindow.destroy

    SDL.quit()

    printfn "All done! Press any key."
    Console.ReadKey()
    |> ignore

    0
