module SDL

open System.Runtime.InteropServices
open System

module private SDLInitNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_Init(uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_Quit()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_InitSubSystem(uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_QuitSubSystem(uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern uint32 SDL_WasInit(uint32 flags)

[<Flags>]
type Init =
    | Timer          = 0x00000001
    | Audio          = 0x00000010
    | Video          = 0x00000020
    | Joystick       = 0x00000200
    | Haptic         = 0x00001000
    | GameController = 0x00002000
    | Events         = 0x00004000
    | Everything     = 0x00007231

type System(flags:Init) =
    do
        SDLInitNative.SDL_Init(flags |> uint32) |> ignore
    member this.initSubSystem (flags: Init) :bool =
        0 = SDLInitNative.SDL_InitSubSystem(flags |> uint32)
    member this.quitSubSystem (flags: Init) :unit =
        SDLInitNative.SDL_QuitSubSystem(flags |> uint32)
    member this.wasInit (flags:Init) :bool =
        flags = (SDLInitNative.SDL_WasInit(flags |> uint32) |> int |> enum<Init>)
    interface IDisposable with
        member this.Dispose() =
            SDLInitNative.SDL_Quit()
