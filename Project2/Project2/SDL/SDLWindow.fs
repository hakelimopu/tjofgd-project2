module SDLWindow

open System.Runtime.InteropServices
open System
open SDLUtility

type Flags = 
    | FullScreen         = 0x00000001
    | OpenGL             = 0x00000002
    | Shown              = 0x00000004
    | Hidden             = 0x00000008
    | Borderless         = 0x00000010
    | Resizable          = 0x00000020
    | Minimized          = 0x00000040
    | Maximized          = 0x00000080
    | InputGrabbed       = 0x00000100
    | InputFocus         = 0x00000200
    | MouseFocus         = 0x00000400
    | FullScreenDesktop  = 0x00001001
    | Foreign            = 0x00000800
    | AllowHighDPI       = 0x00002000
    | MouseCapture       = 0x00004000

type Window = IntPtr

module private SDLWindowNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Window SDL_CreateWindow(IntPtr title, int x, int y, int w, int h, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyWindow(Window window);

let create (title:string) (x:int<px>) (y:int<px>) (w:int<px>) (h:int<px>) (flags:uint32) :Window =
    title
    |> SDLUtility.withUtf8String (fun ptr -> SDLWindowNative.SDL_CreateWindow(ptr, x |> int, y |> int, w |> int, h |> int, flags))
    
let destroy (window:Window) =
    SDLWindowNative.SDL_DestroyWindow(window)