module SDLError

open System.Runtime.InteropServices

module private SDLErrorNative = 
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_ClearError()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern System.IntPtr SDL_GetError()//does not give me the string yet!
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)>]
    extern int SDL_SetError(string fmt)

let set errorString =
    SDLErrorNative.SDL_SetError(errorString)
    |> ignore

let get () =
    SDLErrorNative.SDL_GetError()

let clear () =
    SDLErrorNative.SDL_ClearError()