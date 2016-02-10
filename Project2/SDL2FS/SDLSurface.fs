module SDLSurface

open System
open SDLGeometry
open System.Runtime.InteropServices
open SDLUtility
open Microsoft.FSharp.NativeInterop
open SDLPixel

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

type Surface = SDLUtility.Pointer

module private SDLSurfaceNative =
    //Creating RGB surfaces
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateRGBSurface(uint32 flags, int width, int height, int depth, uint32 Rmask, uint32 Gmask, uint32 Bmask, uint32 Amask)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateRGBSurfaceFrom(IntPtr pixels, int width, int height, int depth, int pitch, uint32 Rmask, uint32 Gmask, uint32 Bmask, uint32 Amask)

    //Clean up surface
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_FreeSurface(IntPtr surface)    

    //Palette
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern  int SDL_SetSurfacePalette(IntPtr surface, IntPtr palette)//TODO

    //Locking
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern  int SDL_LockSurface(IntPtr surface)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern  void SDL_UnlockSurface(IntPtr surface)

    //Bitmaps
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_LoadBMP_RW(IntPtr src, int freesrc)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SaveBMP_RW(IntPtr surface, IntPtr dst, int freedst)

    //RLE
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetSurfaceRLE(IntPtr surface, int flag)

    //Color Key
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetColorKey(IntPtr surface, int flag, uint32 key)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetColorKey(IntPtr surface, uint32 * key)

    //Color mod
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetSurfaceColorMod(IntPtr surface, uint8 r, uint8 g, uint8 b)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetSurfaceColorMod(IntPtr surface, uint8 * r, uint8 * g, uint8 * b)

    //IntPtr Alpha
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetSurfaceAlphaMod(IntPtr surface, uint8 alpha)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetSurfaceAlphaMod(IntPtr surface, uint8 * alpha)

    //Blend Mode
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetSurfaceBlendMode(IntPtr surface, int blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetSurfaceBlendMode(IntPtr surface, int* blendMode)

    //Clip Rect
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int  SDL_SetClipRect(IntPtr surface, SDL_Rect* rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void  SDL_GetClipRect(IntPtr surface, SDL_Rect* rect)

    //Conversions
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_ConvertSurface(IntPtr src, IntPtr fmt, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_ConvertSurfaceFormat(IntPtr src, uint32 pixel_format, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_ConvertPixels(int width, int height, uint32 src_format, IntPtr src, int src_pitch, uint32 dst_format, IntPtr dst, int dst_pitch)

    //filling rectangles
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_FillRect(IntPtr dst, IntPtr rect, uint32 color)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_FillRects(IntPtr dst, IntPtr rects, int count, uint32 color)//TODO

    //blitting
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpperBlit(IntPtr src, IntPtr srcrect, IntPtr dst, IntPtr dstrect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_LowerBlit(IntPtr src, IntPtr srcrect, IntPtr dst, IntPtr dstrect)

    //stretching
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SoftStretch(IntPtr src, IntPtr srcrect, IntPtr dst, IntPtr dstrect)

    //scaled blitting
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpperBlitScaled(IntPtr src, IntPtr srcrect, IntPtr dst, IntPtr dstrect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_LowerBlitScaled(IntPtr src, IntPtr srcrect, IntPtr dst, IntPtr dstrect)


let createRGB (width:int<px>,height:int<px>,depth:int<bit/px>) (rmask:uint32,gmask:uint32,bmask:uint32,amask:uint32) :Surface=
    let ptr = SDLSurfaceNative.SDL_CreateRGBSurface(0u,width/1<px>,height/1<px>,depth/1<bit/px>,rmask,gmask,bmask,amask)
    new SDLUtility.Pointer(ptr, SDLSurfaceNative.SDL_FreeSurface)

let private getFormat (surface:Surface) :IntPtr =
    let sdlSurface = 
        surface.Pointer
        |> NativePtr.ofNativeInt<SDL_Surface>
        |> NativePtr.read
    sdlSurface.format

let fillRect (rect:Rectangle option) (color:SDLPixel.Color) (surface:Surface) :bool =
    let format = surface |> getFormat
    SDLGeometry.withSDLRectPointer (fun r->0 = SDLSurfaceNative.SDL_FillRect(surface.Pointer, r, color |> SDLPixel.mapColor format)) rect

let loadBmp (pixelFormat: uint32) (fileName:string) : Surface =
    let bitmapSurface = SDLSurfaceNative.SDL_LoadBMP_RW(SDLUtility.withUtf8String (fun ptr->SDLRWops.SDLRWopsNative.SDL_RWFromFile(ptr,"rb")) fileName, 1)
    let convertedSurface = SDLSurfaceNative.SDL_ConvertSurfaceFormat(bitmapSurface,pixelFormat,0u)
    SDLSurfaceNative.SDL_FreeSurface bitmapSurface
    new SDLUtility.Pointer(convertedSurface, SDLSurfaceNative.SDL_FreeSurface)

let saveBmp (fileName:string) (surface:Surface) :bool =
    0 = SDLSurfaceNative.SDL_SaveBMP_RW(surface.Pointer, SDLUtility.withUtf8String (fun ptr->SDLRWops.SDLRWopsNative.SDL_RWFromFile(ptr,"wb")) fileName, 1)

let upperBlit (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_UpperBlit(src.Pointer,srcptr,dst.Pointer,dstptr)) dstrect) srcrect

let blit = upperBlit

let lowerBlit (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_LowerBlit(src.Pointer,srcptr,dst.Pointer,dstptr)) dstrect) srcrect

let upperBlitScaled (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_UpperBlitScaled(src.Pointer,srcptr,dst.Pointer,dstptr)) dstrect) srcrect

let lowerBlitScaled (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_LowerBlitScaled(src.Pointer,srcptr,dst.Pointer,dstptr)) dstrect) srcrect

let softStretch (srcrect:Rectangle option) (src:Surface) (dstrect:Rectangle option) (dst:Surface) =
    SDLGeometry.withSDLRectPointer (fun srcptr -> SDLGeometry.withSDLRectPointer (fun dstptr -> 0 = SDLSurfaceNative.SDL_SoftStretch(src.Pointer,srcptr,dst.Pointer,dstptr)) dstrect) srcrect

let setColorKey (color:SDLPixel.Color option) (surface:Surface) =
    let fmt = 
        (surface |> getFormat)
    let key = 
        if color.IsSome then SDLPixel.mapColor fmt color.Value else 0u
    let flag = 
        if color.IsSome then 1 else 0
    0 = SDLSurfaceNative.SDL_SetColorKey(surface.Pointer, flag, key)

let getColorKey (surface:Surface) :SDLPixel.Color option =
    let mutable key: uint32 = 0u
    match SDLSurfaceNative.SDL_GetColorKey(surface.Pointer,&&key) with
    | 0 ->
        let fmt = 
            (surface |> getFormat)
        key |> SDLPixel.getColor fmt |> Some
    | _ -> None

let lockBind (surface:Surface) (func: unit -> unit) :bool =
    if 0 = SDLSurfaceNative.SDL_LockSurface surface.Pointer then
        func()
        SDLSurfaceNative.SDL_UnlockSurface surface.Pointer
        true
    else
        false

let setRLE (surface:Surface) (flag:bool) :bool =
    0 = SDLSurfaceNative.SDL_SetSurfaceRLE(surface.Pointer,(if flag then 1 else 0))


let setModulation (color:SDLPixel.Color) (surface:Surface) :bool = 
    (0 = SDLSurfaceNative.SDL_SetSurfaceColorMod(surface.Pointer, color.Red, color.Green, color.Blue)) && (0 = SDLSurfaceNative.SDL_SetSurfaceAlphaMod(surface.Pointer, color.Alpha))
    
let getModulation (surface:Surface) :SDLPixel.Color option =
    let mutable r : uint8 = 0uy
    let mutable g : uint8 = 0uy
    let mutable b : uint8 = 0uy
    let mutable a : uint8 = 0uy
    let result = SDLSurfaceNative.SDL_GetSurfaceColorMod(surface.Pointer,&&r,&&g,&&b), SDLSurfaceNative.SDL_GetSurfaceAlphaMod(surface.Pointer,&&a)
    match result with
    | (0,0) -> {Red=r;Green=g;Blue=b;Alpha=a} |> Some
    | _ -> None
