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

type Renderer = SDLUtility.Pointer
                                           
module private SDLRenderNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateRenderer(IntPtr window, int index, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyRenderer(IntPtr renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderClear(IntPtr renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetRenderDrawColor(IntPtr renderer, uint8 r, uint8 g, uint8 b, uint8 a)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderPresent(IntPtr rendererr)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetLogicalSize(IntPtr renderer, int w, int h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetNumRenderDrivers()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRenderDriverInfo(int index, IntPtr info)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_CreateWindowAndRenderer(int width, int height, uint32 window_flags, IntPtr window, IntPtr renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateSoftwareRenderer(Surface surface)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetRenderer(IntPtr  window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRendererInfo(IntPtr renderer, IntPtr info)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRendererOutputSize(IntPtr renderer, IntPtr w, IntPtr h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderTargetSupported(IntPtr renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetRenderTarget(IntPtr renderer, IntPtr texture)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetRenderTarget(IntPtr renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderGetLogicalSize(IntPtr renderer, IntPtr w, IntPtr h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetViewport(IntPtr renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderGetViewport(IntPtr renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetClipRect(IntPtr renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderGetClipRect(IntPtr renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderIsClipEnabled(IntPtr renderer)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderSetScale(IntPtr  renderer, float scaleX, float scaleY)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RenderGetScale(IntPtr renderer, IntPtr scaleX, IntPtr scaleY)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRenderDrawColor(IntPtr renderer, IntPtr  r, IntPtr  g, IntPtr  b, IntPtr  a)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetRenderDrawBlendMode(IntPtr renderer, int blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRenderDrawBlendMode(IntPtr renderer, IntPtr blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawPoint(IntPtr renderer, int x, int y)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawPoints(IntPtr renderer, IntPtr   points, int count)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawLine(IntPtr renderer, int x1, int y1, int x2, int y2)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawLines(IntPtr renderer, IntPtr   points, int count)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawRect(IntPtr renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderDrawRects(IntPtr renderer, IntPtr rects, int count)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderFillRect(IntPtr renderer, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderFillRects(IntPtr renderer, IntPtr rects, int count)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr  texture, IntPtr srcrect, IntPtr dstrect, double angle, IntPtr  center, int flip)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_RenderReadPixels(IntPtr  renderer,IntPtr rect,uint32 format,IntPtr pixels, int pitch)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GL_BindTexture(IntPtr texture, IntPtr texw, IntPtr texh)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GL_UnbindTexture(IntPtr texture)

let create (window:SDLWindow.Window) (index:int) (flags:Flags) :Renderer =
    let ptr = SDLRenderNative.SDL_CreateRenderer(window.Pointer, index, flags |> uint32)
    new SDLUtility.Pointer(ptr,SDLRenderNative.SDL_DestroyRenderer)

let clear (renderer:Renderer) :bool =
    0 = SDLRenderNative.SDL_RenderClear(renderer.Pointer)

let present (renderer:Renderer) :unit =
    SDLRenderNative.SDL_RenderPresent(renderer.Pointer)

let setDrawColor (r, g, b, a) (renderer:Renderer) =
    0 = SDLRenderNative.SDL_SetRenderDrawColor(renderer.Pointer,r,g,b,a)

let setLogicalSize (w:int<px>,h:int<px>) (renderer:Renderer) =
    0 = SDLRenderNative.SDL_RenderSetLogicalSize(renderer.Pointer,w |> int,h |> int)

let copy (texture:SDLTexture.Texture) (srcrect:Rectangle option) (dstrect:Rectangle option) (renderer:Renderer) =
    SDLGeometry.withSDLRectPointer(fun src -> SDLGeometry.withSDLRectPointer(fun dst -> 0 = SDLRenderNative.SDL_RenderCopy(renderer.Pointer,texture.Pointer,src,dst)) dstrect) srcrect
    