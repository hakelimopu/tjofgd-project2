module SDLError

#nowarn "9"

open System.Runtime.InteropServices
open System
open SDLUtility

module private SDLErrorNative = 
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_ClearError()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetError()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)>]
    extern int SDL_SetError(IntPtr fmt)

let set (errorString:string) =
    errorString
    |> SDLUtility.withUtf8String (fun ptr -> SDLErrorNative.SDL_SetError(ptr) |> ignore)

let get () =
    SDLErrorNative.SDL_GetError()
    |> intPtrToStringUtf8

let clear () =
    SDLErrorNative.SDL_ClearError()