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

type BitmapOrder =
    | None=0
    | _4321=1
    | _1234=2

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

type ArrayOrder =
    | None=0
    | RGB=1
    | RGBA=2
    | ARGB=3
    | BGR=4
    | BGRA=5
    | ABGR=6


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


let UnknownFormat     = 0
let Index1LSBFormat   = DefinePixelFormat(PixelType.Index1   |> int, BitmapOrder._4321  |> int, 0                     |> int,  1, 0)
let Index1MSBFormat   = DefinePixelFormat(PixelType.Index1   |> int, BitmapOrder._1234  |> int, 0                     |> int,  1, 0)
let Index4LSBFormat   = DefinePixelFormat(PixelType.Index4   |> int, BitmapOrder._4321  |> int, 0                     |> int,  4, 0)
let Index4MSBFormat   = DefinePixelFormat(PixelType.Index4   |> int, BitmapOrder._1234  |> int, 0                     |> int,  4, 0)
let Index8Format      = DefinePixelFormat(PixelType.Index8   |> int, 0                  |> int, 0                     |> int,  8, 1)
let RGB332Format      = DefinePixelFormat(PixelType.Packed8  |> int, PackedOrder.XRGB   |> int, PackedLayout._332     |> int,  8, 1)
let RGB444Format      = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.XRGB   |> int, PackedLayout._4444    |> int, 12, 2)
let RGB555Format      = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.XRGB   |> int, PackedLayout._1555    |> int, 15, 2)
let BGR555Format      = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.XBGR   |> int, PackedLayout._1555    |> int, 15, 2)
let ARGB4444Format    = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.ARGB   |> int, PackedLayout._4444    |> int, 16, 2)
let RGBA4444Format    = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.RGBA   |> int, PackedLayout._4444    |> int, 16, 2)
let ABGR4444Format    = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.ABGR   |> int, PackedLayout._4444    |> int, 16, 2)
let BGRA4444Format    = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.BGRA   |> int, PackedLayout._4444    |> int, 16, 2)
let ARGB1555Format    = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.ARGB   |> int, PackedLayout._1555    |> int, 16, 2)
let RGBA5551Format    = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.RGBA   |> int, PackedLayout._5551    |> int, 16, 2)
let ABGR1555Format    = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.ABGR   |> int, PackedLayout._1555    |> int, 16, 2)
let BGRA5551Format    = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.BGRA   |> int, PackedLayout._5551    |> int, 16, 2)
let RGB565Format      = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.XRGB   |> int, PackedLayout._565     |> int, 16, 2)
let BGR565Format      = DefinePixelFormat(PixelType.Packed16 |> int, PackedOrder.XBGR   |> int, PackedLayout._565     |> int, 16, 2)
let RGB24Format       = DefinePixelFormat(PixelType.ArrayU8  |> int, ArrayOrder.RGB     |> int, 0                     |> int, 24, 3)
let BGR24Format       = DefinePixelFormat(PixelType.ArrayU8  |> int, ArrayOrder.BGR     |> int, 0                     |> int, 24, 3)
let RGB888Format      = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.XRGB   |> int, PackedLayout._8888    |> int, 24, 4)
let RGBX8888Format    = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.RGBX   |> int, PackedLayout._8888    |> int, 24, 4)
let BGR888Format      = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.XBGR   |> int, PackedLayout._8888    |> int, 24, 4)
let BGRX8888Format    = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.BGRX   |> int, PackedLayout._8888    |> int, 24, 4)
let ARGB8888Format    = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.ARGB   |> int, PackedLayout._8888    |> int, 32, 4)
let RGBA8888Format    = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.RGBA   |> int, PackedLayout._8888    |> int, 32, 4)
let ABGR8888Format    = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.ABGR   |> int, PackedLayout._8888    |> int, 32, 4)
let BGRA8888Format    = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.BGRA   |> int, PackedLayout._8888    |> int, 32, 4)
let ARGB2101010Format = DefinePixelFormat(PixelType.Packed32 |> int, PackedOrder.ARGB   |> int, PackedLayout._2101010 |> int, 32, 4)

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
        val colors: IntPtr
        val version: uint32
        val refcount: int
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





