module SDLTexture

#nowarn "9"

open System.Runtime.InteropServices
open System
open SDLUtility
open Microsoft.FSharp.NativeInterop

type Access =
    | Static    = 0
    | Streaming = 1
    | Target    = 2

[<Flags>]
type Modulate = 
    | None  = 0x00000000
    | Color = 0x00000001
    | Alpha = 0x00000002

type Texture = IntPtr

module private SDLTextureNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Texture SDL_CreateTexture(IntPtr renderer, uint32 format, int access, int w, int h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyTexture(Texture texture);    
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Texture SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_QueryTexture(Texture texture, IntPtr format, IntPtr access, IntPtr w, IntPtr h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetTextureColorMod(Texture texture, uint8 r, uint8 g, uint8 b)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetTextureColorMod(Texture texture, IntPtr  r, IntPtr  g, IntPtr  b)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetTextureAlphaMod(Texture texture, uint8 alpha)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetTextureAlphaMod(Texture texture, IntPtr  alpha)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetTextureBlendMode(Texture texture, int blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetTextureBlendMode(Texture texture, IntPtr blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpdateTexture(Texture texture, IntPtr  rect, IntPtr pixels, int pitch)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpdateYUVTexture(Texture texture, IntPtr  rect, IntPtr Yplane, int Ypitch, IntPtr Uplane, int Upitch, IntPtr Vplane, int Vpitch)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_LockTexture(Texture texture, IntPtr  rect, IntPtr pixels, IntPtr pitch)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_UnlockTexture(Texture texture)

let create format (access: Access) (w: int<px>,h: int<px>) renderer =
    SDLTextureNative.SDL_CreateTexture(renderer,format,access |> int,w / 1<px>,h / 1<px>)

let fromSurface renderer surface =
    SDLTextureNative.SDL_CreateTextureFromSurface(renderer,surface)

let destroy texture =
    SDLTextureNative.SDL_DestroyTexture(texture)

let update (dstrect:SDLGeometry.Rectangle option) (src:SDLSurface.Surface) (texture:Texture) : bool =
    let mutable sdlRect = 
        dstrect
        |> Option.map SDLGeometry.rectangleToSDL_Rect
    let rectptr =
        if sdlRect.IsNone then
            IntPtr.Zero
        else
            let mutable temp = sdlRect.Value
            NativePtr.toNativeInt &&temp
    let surf =
        src
        |> NativePtr.ofNativeInt<SDLSurface.SDL_Surface>
        |> NativePtr.read
    0 = SDLTextureNative.SDL_UpdateTexture(texture,rectptr,surf.pixels,surf.pitch)
