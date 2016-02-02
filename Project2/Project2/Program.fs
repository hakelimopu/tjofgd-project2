open System.Runtime.InteropServices
open System

let rec eventPump window renderer texture =
    renderer |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) |> ignore
    renderer |> SDLRender.clear |> ignore
    renderer |> SDLRender.copy texture IntPtr.Zero IntPtr.Zero |> ignore
    renderer |> SDLRender.present 
    let event = SDLEvent.waitEvent()
    match event with
    | Some (SDLEvent.MouseButtonUp x) -> ()
    | Some _ -> eventPump window renderer texture
    | None -> ()

[<EntryPoint>]
let main argv = 
    let version = SDLVersion.getVersion()

    (version.Major, version.Minor, version.Patch)
    |||> printfn "SDL Version = %i.%i.%i"

    SDL.init(SDL.SDL_INIT_VIDEO ||| SDL.SDL_INIT_EVENTS)
    |> printfn "Init Result = %b"

    let mainWindow = SDLWindow.create "test" 0 0 640 480 0u

    let mainRenderer = SDLRender.create mainWindow -1 SDLRender.SDL_RENDERER_ACCELERATED

    let mainTexture = mainRenderer |> SDLTexture.create SDLPixel.SDL_PIXELFORMAT_ARGB8888 SDLTexture.SDL_TEXTUREACCESS_STREAMING (160,120)

    mainRenderer |> SDLRender.setLogicalSize (160,120) |> ignore

    eventPump mainWindow mainRenderer mainTexture

    mainTexture |> SDLTexture.destroy

    mainRenderer |> SDLRender.destroy 

    mainWindow |> SDLWindow.destroy

    SDL.quit()

    printfn "All done! Press any key."
    Console.ReadKey()
    |> ignore

    0
