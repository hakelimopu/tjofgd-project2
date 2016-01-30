module SDLVersion

#nowarn "9" //Dear Compiler, Quiet you! Love, Me.

open System.Runtime.InteropServices

[<StructLayout(LayoutKind.Sequential)>]
type private SDL_version =
    struct
        val major: uint8
        val minor: uint8
        val patch: uint8
    end

module private SDLVersionNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_GetVersion(SDL_version& ver)    

type Version = {Major: uint8; Minor: uint8; Patch: uint8}

let getVersion () =
    let mutable version = new SDL_version()
    SDLVersionNative.SDL_GetVersion(&version)
    {Major = version.major; Minor=version.minor; Patch = version.patch}
