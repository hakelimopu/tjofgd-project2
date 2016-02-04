module SDLRender

#nowarn "9"

open System.Runtime.InteropServices
open System
open SDLUtility
open SDLGeometry
open SDLTexture
open SDLSurface

[<Flags>]
type Flags = 
    | Software      = 0x00000001
    | Accelerated   = 0x00000002
    | PresentVSync  = 0x00000004
    | TargetTexture = 0x00000008

[<Flags>]
type Flip =
    | None = 0x00000000
    | Horizontal = 0x00000001
    | Vertical = 0x00000002
                                           
type Renderer = IntPtr

module private SDLRenderNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Renderer SDL_CreateRenderer(SDLWindow.Window window, int index, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyRenderer(Renderer renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderClear(Renderer renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetRenderDrawColor(Renderer renderer, uint8 r, uint8 g, uint8 b, uint8 a)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderPresent(Renderer rendererr)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetLogicalSize(Renderer renderer, int w, int h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderCopy(Renderer renderer, SDLTexture.Texture texture, SDL_Rect& srcrect, SDL_Rect& dstrect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetNumRenderDrivers()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRenderDriverInfo(int index, IntPtr info)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_CreateWindowAndRenderer(int width, int height, uint32 window_flags, IntPtr window, IntPtr renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Renderer SDL_CreateSoftwareRenderer(Surface surface)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Renderer SDL_GetRenderer(IntPtr  window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRendererInfo(Renderer renderer, IntPtr info)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRendererOutputSize(Renderer renderer, IntPtr w, IntPtr h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderTargetSupported(Renderer renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetRenderTarget(Renderer renderer, Texture texture)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Texture  SDL_GetRenderTarget(Renderer renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderGetLogicalSize(Renderer renderer, IntPtr w, IntPtr h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetViewport(Renderer renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderGetViewport(Renderer renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetClipRect(Renderer renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderGetClipRect(Renderer renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderIsClipEnabled(Renderer renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetScale(Renderer  renderer, float scaleX, float scaleY)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderGetScale(Renderer renderer, IntPtr scaleX, IntPtr scaleY)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRenderDrawColor(Renderer renderer, IntPtr  r, IntPtr  g, IntPtr  b, IntPtr  a)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetRenderDrawBlendMode(Renderer renderer, int blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRenderDrawBlendMode(Renderer renderer, IntPtr blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawPoint(Renderer renderer, int x, int y)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawPoints(Renderer renderer, IntPtr   points, int count)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawLine(Renderer renderer, int x1, int y1, int x2, int y2)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawLines(Renderer renderer, IntPtr   points, int count)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawRect(Renderer renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawRects(Renderer renderer, IntPtr rects, int count)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderFillRect(Renderer renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderFillRects(Renderer renderer, IntPtr rects, int count)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderCopyEx(Renderer renderer, Texture  texture, IntPtr srcrect, IntPtr dstrect, double angle, IntPtr  center, int flip)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderReadPixels(Renderer  renderer,IntPtr rect,uint32 format,IntPtr pixels, int pitch)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GL_BindTexture(Texture texture, IntPtr texw, IntPtr texh)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GL_UnbindTexture(Texture texture)

let create (window:SDLWindow.Window) (index:int) (flags:Flags) :Renderer =
    SDLRenderNative.SDL_CreateRenderer(window, index, flags |> uint32)

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