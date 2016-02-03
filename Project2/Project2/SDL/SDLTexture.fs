module SDLTexture

open System.Runtime.InteropServices
open System
open SDLUtility

type Access =
    | Static    = 0
    | Streaming = 1
    | Target    = 2

type Texture = IntPtr

module private SDLTextureNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Texture SDL_CreateTexture(IntPtr renderer, uint32 format, int access, int w, int h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyTexture(Texture texture);    
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Texture SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);

let create format (access: Access) (w: int<px>,h: int<px>) renderer =
    SDLTextureNative.SDL_CreateTexture(renderer,format,access |> int,w / 1<px>,h / 1<px>)

let fromSurface renderer surface =
    SDLTextureNative.SDL_CreateTextureFromSurface(renderer,surface)

let destroy texture =
    SDLTextureNative.SDL_DestroyTexture(texture)