module SDLSurface

open System
open SDLGeometry
open System.Runtime.InteropServices

#nowarn "9"

[<StructLayout(LayoutKind.Sequential)>]
type SDL_Surface =
    struct
        val flags :uint32
        val format: IntPtr//SDL_PixelFormat
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


