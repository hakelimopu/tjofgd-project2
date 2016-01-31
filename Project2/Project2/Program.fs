open System.Runtime.InteropServices
open System

let rec eventPump window renderer =
    let event = SDLEvent.waitEvent()
    match event with
    | Some x -> 
        if x.[0] = 2uy && x.[1] = 4uy then
            ()
        else
            eventPump window renderer
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

    eventPump mainWindow mainRenderer

    mainRenderer |> SDLRender.destroy 

    mainWindow |> SDLWindow.destroy

    SDL.quit()

    printfn "All done! Press any key."
    Console.ReadKey()
    |> ignore

    0
