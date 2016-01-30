open System.Runtime.InteropServices

[<EntryPoint>]
let main argv = 
    let version = SDLVersion.getVersion()

    (version.Major, version.Minor, version.Patch)
    |||> printfn "SDL Version = %i.%i.%i"

    SDL.init(SDL.SDL_INIT_VIDEO ||| SDL.SDL_INIT_EVENTS)
    |> printfn "Init Result = %b"

    let mainWindow = SDLWindow.create "test" 0 0 640 480 (4 |> uint32)

    SDLWindow.destroy mainWindow

    SDL.quit()

    System.Console.ReadKey()
    |> ignore

    0
