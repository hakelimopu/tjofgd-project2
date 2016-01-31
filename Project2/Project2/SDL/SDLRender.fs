module SDLRender

open System.Runtime.InteropServices
open System

let SDL_RENDERER_SOFTWARE      = 0x00000001u     (* The renderer is a software fallback *)
let SDL_RENDERER_ACCELERATED   = 0x00000002u     (* The renderer uses hardware
                                                    acceleration *)
let SDL_RENDERER_PRESENTVSYNC  = 0x00000004u     (* Present is synchronized
                                                    with the refresh rate *)
let SDL_RENDERER_TARGETTEXTURE = 0x00000008u     (* The renderer supports
                                                    rendering to texture *)

type Renderer = IntPtr

module SDLRenderNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Renderer SDL_CreateRenderer(SDLWindow.Window window, int index, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyRenderer(Renderer renderer)

let create (window:SDLWindow.Window) (index:int) (flags:uint32) :Renderer =
    SDLRenderNative.SDL_CreateRenderer(window, index, flags)

let destroy (renderer:Renderer) :unit =
    SDLRenderNative.SDL_DestroyRenderer(renderer)