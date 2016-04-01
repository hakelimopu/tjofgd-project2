namespace SDL

#nowarn "9"

open System.Runtime.InteropServices
open System

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute>]
module Error = 

    module private SDLErrorNative = 
        [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
        extern void SDL_ClearError()
        [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
        extern IntPtr SDL_GetError()
        [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)>]
        extern int SDL_SetError(IntPtr fmt)

    let set (errorString:string) =
        errorString
        |> SDL.Utility.withUtf8String (fun ptr -> SDLErrorNative.SDL_SetError(ptr) |> ignore)

    let get () =
        SDLErrorNative.SDL_GetError()
        |> SDL.Utility.intPtrToStringUtf8

    let clear () =
        SDLErrorNative.SDL_ClearError()