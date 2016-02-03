module SDLSurface

open System
open SDLGeometry
open System.Runtime.InteropServices
open SDLUtility
open Microsoft.FSharp.NativeInterop

#nowarn "9"

[<StructLayout(LayoutKind.Sequential)>]
type SDL_Surface =
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
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Surface SDL_CreateRGBSurface(uint32 flags, int width, int height, int depth, uint32 Rmask, uint32 Gmask, uint32 Bmask, uint32 Amask)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_FreeSurface(Surface surface);    

let createRGB (width:int<px>,height:int<px>,depth:int<bit/px>) (rmask:uint32,gmask:uint32,bmask:uint32,amask:uint32) =
    let ptr = SDLSurfaceNative.SDL_CreateRGBSurface(0u,width/1<px>,height/1<px>,depth/1<bit/px>,rmask,gmask,bmask,amask)
    let psurf = ptr |> NativePtr.ofNativeInt<SDL_Surface>
    let surf = psurf |> NativePtr.read
    let ppixels = surf.pixels |> NativePtr.ofNativeInt<uint32>
    0xFFFFFFFFu |> NativePtr.write ppixels
    ptr

let free (surface:Surface) :unit =
    SDLSurfaceNative.SDL_FreeSurface(surface)