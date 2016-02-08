module SDLVersion

#nowarn "9" 

open System.Runtime.InteropServices
open System

[<StructLayout(LayoutKind.Sequential)>]
type internal SDL_version =
    struct
        val major: uint8
        val minor: uint8
        val patch: uint8
    end

module private SDLVersionNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_GetVersion(SDL_version& ver)    
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)>]
    extern IntPtr SDL_GetRevision()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRevisionNumber()

type Version = {Major: uint8; Minor: uint8; Patch: uint8}

let getVersion () :Version =
    let mutable version = new SDL_version()
    SDLVersionNative.SDL_GetVersion(&version)
    {Major = version.major; Minor=version.minor; Patch = version.patch}

let getRevision () :string =
    SDLVersionNative.SDL_GetRevision()
    |> SDLUtility.intPtrToStringUtf8

let getRevisionNumber() :int =
    SDLVersionNative.SDL_GetRevisionNumber()