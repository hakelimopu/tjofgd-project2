module SDLTexture

open System.Runtime.InteropServices
open System
open SDLUtility

let SDL_TEXTUREACCESS_STATIC    = 0
let SDL_TEXTUREACCESS_STREAMING = 1
let SDL_TEXTUREACCESS_TARGET    = 2

type Texture = IntPtr

module private SDLTextureNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Texture SDL_CreateTexture(IntPtr renderer, uint32 format, int access, int w, int h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyTexture(Texture texture);    

let create format access (w: int<px>,h: int<px>) renderer =
    SDLTextureNative.SDL_CreateTexture(renderer,format,access,w |> int,h |> int)

let destroy texture =
    SDLTextureNative.SDL_DestroyTexture(texture)