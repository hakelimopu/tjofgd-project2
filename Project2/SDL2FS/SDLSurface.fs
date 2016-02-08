module SDLSurface

open System
open SDLGeometry
open System.Runtime.InteropServices
open SDLUtility
open Microsoft.FSharp.NativeInterop

#nowarn "9"

[<StructLayout(LayoutKind.Sequential)>]
type internal SDL_Surface =
    struct
        val flags :uint32
        val format: IntPtr//SDL_PixelFormat*
        val w:int
        val h:int
        val pitch:int
        val pixels: IntPtr
        val userdate: IntPtr
        val locked: int
        val lock_data: IntPtr
        val clip_rect: SDL_Rect
        val map: IntPtr
        val refcount: int
    end

type Surface = IntPtr

module private SDLSurfaceNative =
    //Creating RGB surfaces
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Surface SDL_CreateRGBSurface(uint32 flags, int width, int height, int depth, uint32 Rmask, uint32 Gmask, uint32 Bmask, uint32 Amask)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Surface SDL_CreateRGBSurfaceFrom(IntPtr pixels, int width, int height, int depth, int pitch, uint32 Rmask, uint32 Gmask, uint32 Bmask, uint32 Amask)

    //Clean up surface
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_FreeSurface(Surface surface)    

    //Palette
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern  int SDL_SetSurfacePalette(Surface surface, IntPtr palette)

    //Locking
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern  int SDL_LockSurface(Surface surface)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern  void SDL_UnlockSurface(Surface surface)

    //Bitmaps
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Surface SDL_LoadBMP_RW(IntPtr src, int freesrc)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SaveBMP_RW(Surface surface, IntPtr dst, int freedst)

    //RLE
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetSurfaceRLE(Surface surface, int flag)

    //Color Key
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetColorKey(Surface surface, int flag, uint32 key)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetColorKey(Surface surface, uint32 * key)

    //Color mod
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetSurfaceColorMod(Surface surface, uint8 r, uint8 g, uint8 b)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetSurfaceColorMod(Surface surface, uint8 * r, uint8 * g, uint8 * b)

    //Surface Alpha
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetSurfaceAlphaMod(Surface surface, uint8 alpha)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetSurfaceAlphaMod(Surface surface, uint8 * alpha)

    //Blend Mode
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetSurfaceBlendMode(Surface surface, int blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetSurfaceBlendMode(Surface surface, IntPtr blendMode)

    //Clip Rect
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int  SDL_SetClipRect(Surface surface, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void  SDL_GetClipRect(Surface surface, SDL_Rect * rect)

    //Conversions
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Surface SDL_ConvertSurface(Surface src, IntPtr fmt, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Surface SDL_ConvertSurfaceFormat(Surface src, uint32 pixel_format, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_ConvertPixels(int width, int height, uint32 src_format, IntPtr src, int src_pitch, uint32 dst_format, IntPtr dst, int dst_pitch)

    //filling rectangles
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_FillRect(Surface dst, IntPtr rect, uint32 color)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_FillRects(Surface dst, IntPtr rects, int count, uint32 color)

    //blitting
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpperBlit(Surface src, IntPtr srcrect, Surface dst, IntPtr dstrect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_LowerBlit(Surface src, IntPtr srcrect, Surface dst, IntPtr dstrect)

    //stretching
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SoftStretch(Surface src, IntPtr srcrect, Surface dst, IntPtr dstrect)

    //scaled blitting
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpperBlitScaled(Surface src, IntPtr srcrect, Surface dst, IntPtr dstrect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_LowerBlitScaled(Surface src, IntPtr srcrect, Surface dst, IntPtr dstrect)


let createRGB (width:int<px>,height:int<px>,depth:int<bit/px>) (rmask:uint32,gmask:uint32,bmask:uint32,amask:uint32) =
    SDLSurfaceNative.SDL_CreateRGBSurface(0u,width/1<px>,height/1<px>,depth/1<bit/px>,rmask,gmask,bmask,amask)

let free (surface:Surface) :unit =
    SDLSurfaceNative.SDL_FreeSurface(surface)

let private getFormat (surface:Surface) :IntPtr =
    let sdlSurface = 
        surface
        |> NativePtr.ofNativeInt<SDL_Surface>
        |> NativePtr.read
    sdlSurface.format

let fillRect (rect:Rectangle option) (color:SDLPixel.Color) (surface:Surface) :bool =
    let format = surface |> getFormat
    SDLGeometry.withSDLRectPointer (fun r->0 = SDLSurfaceNative.SDL_FillRect(surface, r, color |> SDLPixel.mapColor format)) rect

let loadBmp (pixelFormat: uint32) (fileName:string) : Surface =
    let bitmapSurface = SDLSurfaceNative.SDL_LoadBMP_RW(SDLUtility.withUtf8String (fun ptr->SDLRWops.SDLRWopsNative.SDL_RWFromFile(ptr,"rb")) fileName, 1)
    let convertedSurface = SDLSurfaceNative.SDL_ConvertSurfaceFormat(bitmapSurface,pixelFormat,0u)
    SDLSurfaceNative.SDL_FreeSurface bitmapSurface
    convertedSurface

let upperBlit (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_UpperBlit(src,srcptr,dst,dstptr)) dstrect) srcrect

let blit = upperBlit

let lowerBlit (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_LowerBlit(src,srcptr,dst,dstptr)) dstrect) srcrect

let upperBlitScaled (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_UpperBlitScaled(src,srcptr,dst,dstptr)) dstrect) srcrect

let lowerBlitScaled (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_LowerBlitScaled(src,srcptr,dst,dstptr)) dstrect) srcrect

let softStretch (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_SoftStretch(src,srcptr,dst,dstptr)) dstrect) srcrect

    
