module SDLPixel

open System.Runtime.InteropServices
open System

#nowarn "9"

type PixelType =
    | Unknown=0
    | Index1=1
    | Index4=2
    | Index8=3
    | Packed8=4
    | Packed16=5
    | Packed32=6
    | ArrayU8=7
    | ArrayU16=8
    | ArrayU32=9
    | ArrayF16=10
    | ArrayF32=11

type PackedOrder =
    | None=0
    | XRGB=1
    | RGBX=2
    | ARGB=3
    | RGBA=4
    | XBGR=5
    | BGRX=6
    | ABGR=7
    | BGRA=8

type PackedLayout =
    | None=0
    | _332=1
    | _4444=2
    | _1555=3
    | _5551=4
    | _565=5
    | _8888=6
    | _2101010=7
    | _1010102=9

let private DefinePixelFormat (typ, order, layout, bits, bytes) =
    ((1 <<< 28) ||| ((typ) <<< 24) ||| ((order) <<< 20) ||| ((layout) <<< 16) ||| ((bits) <<< 8) ||| ((bytes) <<< 0)) |> uint32

let ARGB8888 =
    DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.ARGB |> int, PackedLayout._8888 |> int, 32, 4)

[<StructLayout(LayoutKind.Sequential)>]
type SDL_Color =
    struct
        val r: uint8
        val g: uint8
        val b: uint8
        val a: uint8
    end

[<StructLayout(LayoutKind.Sequential)>]
type SDL_Palette =
    struct
        val ncolors: int
        val colors: IntPtr //SDL_Color*
        val version: uint32;
        val refcount: int;
    end


[<StructLayout(LayoutKind.Sequential)>]
type SDL_PixelFormat =
    struct
        val format: uint32
        val palette: IntPtr//SDL_Palette*
        val BitsPerPixel: uint8
        val BytesPerPixel: uint8
        val padding: uint16
        val Rmask: uint32
        val Gmask: uint32
        val Bmask: uint32
        val Amask: uint32
        val Rloss: uint8
        val Gloss: uint8
        val Bloss: uint8
        val Aloss: uint8
        val Rshift: uint8
        val Gshift: uint8
        val Bshift: uint8
        val Ashift: uint8
        val refcount: int
        val next: IntPtr;//SDL_PixelFormat*
    end





