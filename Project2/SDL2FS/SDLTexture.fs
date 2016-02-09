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

type Texture = SDLUtility.Pointer

module private SDLTextureNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateTexture(IntPtr renderer, uint32 format, int access, int w, int h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyTexture(IntPtr texture);    
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_QueryTexture(IntPtr texture, IntPtr format, IntPtr access, IntPtr w, IntPtr h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetTextureColorMod(IntPtr texture, uint8 r, uint8 g, uint8 b)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetTextureColorMod(IntPtr texture, IntPtr  r, IntPtr  g, IntPtr  b)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetTextureAlphaMod(IntPtr texture, uint8 alpha)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetTextureAlphaMod(IntPtr texture, IntPtr  alpha)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetTextureBlendMode(IntPtr texture, int blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetTextureBlendMode(IntPtr texture, IntPtr blendMode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpdateTexture(IntPtr texture, IntPtr  rect, IntPtr pixels, int pitch)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpdateYUVTexture(IntPtr texture, IntPtr  rect, IntPtr Yplane, int Ypitch, IntPtr Uplane, int Upitch, IntPtr Vplane, int Vpitch)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_LockTexture(IntPtr texture, IntPtr  rect, IntPtr pixels, IntPtr pitch)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_UnlockTexture(IntPtr texture)


let create format (access: Access) (w: int<px>,h: int<px>) (renderer:SDLUtility.Pointer) =
    let ptr = SDLTextureNative.SDL_CreateTexture(renderer.Pointer,format,access |> int,w / 1<px>,h / 1<px>)
    new SDLUtility.Pointer(ptr, SDLTextureNative.SDL_DestroyTexture)

let fromSurface (renderer:SDLUtility.Pointer) surface =
    let ptr = SDLTextureNative.SDL_CreateTextureFromSurface(renderer.Pointer,surface)
    new SDLUtility.Pointer(ptr, SDLTextureNative.SDL_DestroyTexture)

let update (dstrect:SDLGeometry.Rectangle option) (src:SDLSurface.Surface) (texture:Texture) : bool =
    dstrect
    |> SDLGeometry.withSDLRectPointer (fun rectptr->
        let surf =
            src.Pointer
            |> NativePtr.ofNativeInt<SDLSurface.SDL_Surface>
            |> NativePtr.read
        0 = SDLTextureNative.SDL_UpdateTexture(texture.Pointer,rectptr,surf.pixels,surf.pitch)) 
