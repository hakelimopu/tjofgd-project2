module SDLWindow

open System.Runtime.InteropServices
open System

let SDL_WINDOW_FULLSCREEN         = 0x00000001u      (* fullscreen window *)
let SDL_WINDOW_OPENGL             = 0x00000002u      (* window usable with OpenGL context *)
let SDL_WINDOW_SHOWN              = 0x00000004u      (* window is visible *)
let SDL_WINDOW_HIDDEN             = 0x00000008u      (* window is not visible *)
let SDL_WINDOW_BORDERLESS         = 0x00000010u      (* no window decoration *)
let SDL_WINDOW_RESIZABLE          = 0x00000020u      (* window can be resized *)
let SDL_WINDOW_MINIMIZED          = 0x00000040u      (* window is minimized *)
let SDL_WINDOW_MAXIMIZED          = 0x00000080u      (* window is maximized *)
let SDL_WINDOW_INPUT_GRABBED      = 0x00000100u      (* window has grabbed input focus *)
let SDL_WINDOW_INPUT_FOCUS        = 0x00000200u      (* window has input focus *)
let SDL_WINDOW_MOUSE_FOCUS        = 0x00000400u      (* window has mouse focus *)
let SDL_WINDOW_FULLSCREEN_DESKTOP = ( SDL_WINDOW_FULLSCREEN ||| 0x00001000u )
let SDL_WINDOW_FOREIGN            = 0x00000800u      (* window not created by SDL *)
let SDL_WINDOW_ALLOW_HIGHDPI      = 0x00002000u      (* window should be created in high-DPI mode if supported *)
let SDL_WINDOW_MOUSE_CAPTURE      = 0x00004000u      (* window has mouse captured (unrelated to INPUT_GRABBED) *)

type Window = IntPtr

module private SDLWindowNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern Window SDL_CreateWindow(IntPtr title, int x, int y, int w, int h, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyWindow(Window window);

let create (title:string) (x:int) (y:int) (w:int) (h:int) (flags:uint32) :Window =
    title
    |> SDLUtility.withUtf8String (fun ptr -> SDLWindowNative.SDL_CreateWindow(ptr, x, y, w, h, flags))
    
let destroy (window:Window) =
    SDLWindowNative.SDL_DestroyWindow(window)