module SDLRender

#nowarn "9"

open System.Runtime.InteropServices
open System
open SDLUtility
open SDLGeometry

let SDL_RENDERER_SOFTWARE      = 0x00000001u     (* The renderer is a software fallback *)
let SDL_RENDERER_ACCELERATED   = 0x00000002u     (* The renderer uses hardware
                                                    acceleration *)
let SDL_RENDERER_PRESENTVSYNC  = 0x00000004u     (* Present is synchronized
                                                    with the refresh rate *)
let SDL_RENDERER_TARGETTEXTURE = 0x00000008u     (* The renderer supports
                                                    rendering to texture *)
type Renderer = IntPtr

module private SDLRenderNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Renderer SDL_CreateRenderer(SDLWindow.Window window, int index, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyRenderer(Renderer renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderClear(Renderer renderer);
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetRenderDrawColor(Renderer renderer, uint8 r, uint8 g, uint8 b, uint8 a);
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderPresent(Renderer rendererr);
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetLogicalSize(Renderer renderer, int w, int h);
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderCopy(Renderer renderer, SDLTexture.Texture texture, SDL_Rect& srcrect, SDL_Rect& dstrect);

let create (window:SDLWindow.Window) (index:int) (flags:uint32) :Renderer =
    SDLRenderNative.SDL_CreateRenderer(window, index, flags)

let destroy (renderer:Renderer) :unit =
    SDLRenderNative.SDL_DestroyRenderer(renderer)

let clear (renderer:Renderer) :bool =
    0 = SDLRenderNative.SDL_RenderClear(renderer)

let present (renderer:Renderer) :unit =
    SDLRenderNative.SDL_RenderPresent(renderer)

let setDrawColor (r, g, b, a) (renderer:Renderer) =
    0 = SDLRenderNative.SDL_SetRenderDrawColor(renderer,r,g,b,a)

let setLogicalSize (w:int<px>,h:int<px>) (renderer:Renderer) =
    0 = SDLRenderNative.SDL_RenderSetLogicalSize(renderer,w |> int,h |> int)

//TODO: this function does not allow passing of null as the rectangles...
let copy texture (srcrect:Rectangle) (dstrect:Rectangle) (renderer:Renderer) =
    let mutable src = rectangleToSDL_Rect srcrect
    let mutable dst = rectangleToSDL_Rect dstrect
    0 = SDLRenderNative.SDL_RenderCopy(renderer,texture,&src,&dst)