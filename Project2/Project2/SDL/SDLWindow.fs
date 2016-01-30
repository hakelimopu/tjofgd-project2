module SDLWindow

open System.Runtime.InteropServices
open System

type Window = IntPtr

module private SDLWindowNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateWindow(IntPtr title, int x, int y, int w, int h, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyWindow(IntPtr window);

let create (title:string) (x:int) (y:int) (w:int) (h:int) (flags:uint32) :Window =
    title
    |> SDLUtility.withUtf8String (fun ptr -> SDLWindowNative.SDL_CreateWindow(ptr, x, y, w, h, flags))
    
let destroy (window:Window) =
    SDLWindowNative.SDL_DestroyWindow(window)