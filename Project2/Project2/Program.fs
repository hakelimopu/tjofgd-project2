open System.Runtime.InteropServices

[<EntryPoint>]
let main argv = 
    let version = SDLVersion.getVersion()

    (version.Major, version.Minor, version.Patch)
    |||> printfn "SDL Version = %i.%i.%i"

    SDL.init(SDL.SDL_INIT_VIDEO ||| SDL.SDL_INIT_EVENTS)
    |> printfn "Init Result = %b"

    let error = SDLError.get()

    SDLError.set "test"
    let error2 = SDLError.get()

    SDLError.clear()
    let error3 = SDLError.get()

    SDL.quit()

    System.Console.ReadKey()
    |> ignore

    0
