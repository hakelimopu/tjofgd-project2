module SDLEvent

open System.Runtime.InteropServices
open System

let SDL_QUIT = 0x100u

module private SDLEventNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_WaitEvent(IntPtr event);

let waitEvent () =
    let mutable bytes = [| for i in 0..55 -> 0uy |]
    let pinnedArray = GCHandle.Alloc(bytes, GCHandleType.Pinned)
    let result = SDLEventNative.SDL_WaitEvent(pinnedArray.AddrOfPinnedObject())
    pinnedArray.Free()
    if result=1 then
        Some bytes
    else
        None
