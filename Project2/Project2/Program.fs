open SDLUtility
open SDLGeometry
open System

let rec eventPump window renderer texture surface bitmap =
    let screenSpace = {X=0<px>;Y=0<px>;Width=160<px>;Height=120<px>}
    renderer |> SDLRender.setDrawColor (255uy,0uy,255uy,255uy) |> ignore
    renderer |> SDLRender.clear |> ignore

    surface
    |> SDLSurface.fillRect {X=0<px>;Y=0<px>;Width=10<px>;Height=10<px>} 0xFFFF0000u
    |> ignore

    surface
    |> SDLSurface.fillRect {X=10<px>;Y=10<px>;Width=10<px>;Height=10<px>} 0xFF0000FFu
    |> ignore

    texture
    |> SDLTexture.update None bitmap
    |> ignore

    renderer |> SDLRender.copy texture screenSpace screenSpace |> ignore
    renderer |> SDLRender.present 

    match SDLEvent.waitEvent() with
    | Some (SDLEvent.Quit x) -> ()
    | Some (SDLEvent.KeyDown x) -> if x.Keysym.Sym = 0x1B then () else eventPump window renderer texture surface bitmap
    | Some _ -> eventPump window renderer texture surface bitmap
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

    SDLPixel.RGBA8888Format 
    |> SDLPixel.formatEnumName
    |> printfn "Format Name = %s"

    let mainWindow = SDLWindow.create "test" 100<px> 100<px> 640<px> 480<px> 0u

    let mainRenderer = SDLRender.create mainWindow -1 SDLRender.Flags.Accelerated

    let surface = SDLSurface.createRGB (160<px>,120<px>,32<bit/px>) (0x00FF0000u,0x0000FF00u,0x000000FFu,0xFF000000u)

    let bitmap = SDLSurface.loadBmp "Content/smile.bmp"

    surface |> SDLSurface.fillRect {X=0<px>;Y=0<px>;Width=160<px>;Height=120<px>} 0xFF000000u |> ignore
    surface |> SDLSurface.fillRect {X=0<px>;Y=0<px>;Width=8<px>;Height=8<px>} 0xFFFFFFFFu |> ignore
   
    let mainTexture = mainRenderer |> SDLTexture.create SDLPixel.ARGB8888Format SDLTexture.Access.Streaming (160<px>,120<px>)

    mainRenderer |> SDLRender.setLogicalSize (160<px>,120<px>) |> ignore

    eventPump mainWindow mainRenderer mainTexture surface bitmap

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
