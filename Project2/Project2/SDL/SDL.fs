module SDL

open System.Runtime.InteropServices

module private SDLInitNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_Init(uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_Quit()

let SDL_INIT_TIMER          = 0x00000001 |> uint32
let SDL_INIT_AUDIO          = 0x00000010 |> uint32
let SDL_INIT_VIDEO          = 0x00000020 |> uint32  (* SDL_INIT_VIDEO implies SDL_INIT_EVENTS *)
let SDL_INIT_JOYSTICK       = 0x00000200 |> uint32  (*  SDL_INIT_JOYSTICK implies SDL_INIT_EVENTS *)
let SDL_INIT_HAPTIC         = 0x00001000 |> uint32
let SDL_INIT_GAMECONTROLLER = 0x00002000 |> uint32  (*  SDL_INIT_GAMECONTROLLER implies SDL_INIT_JOYSTICK *)
let SDL_INIT_EVENTS         = 0x00004000 |> uint32
let SDL_INIT_EVERYTHING     = (SDL_INIT_TIMER ||| SDL_INIT_AUDIO ||| SDL_INIT_VIDEO ||| SDL_INIT_EVENTS ||| SDL_INIT_JOYSTICK ||| SDL_INIT_HAPTIC ||| SDL_INIT_GAMECONTROLLER)

let init (flags: uint32) :bool=
    SDLInitNative.SDL_Init(flags) = 0

let quit () =
    SDLInitNative.SDL_Quit()