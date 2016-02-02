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

let wasInit (flags:Init) :Init =
    SDLInitNative.SDL_WasInit(flags |> uint32) |> int |> enum<Init>

let init (flags: Init) :bool =
    0 = SDLInitNative.SDL_Init(flags |> uint32)

let initSubSystem (flags: Init) :bool =
    0 = SDLInitNative.SDL_InitSubSystem(flags |> uint32)

let quitSubSystem (flags: Init) :unit =
    SDLInitNative.SDL_QuitSubSystem(flags |> uint32)

let quit () : unit=
    SDLInitNative.SDL_Quit()

