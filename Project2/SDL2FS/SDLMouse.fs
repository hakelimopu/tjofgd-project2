module SDLMouse

open System
open System.Runtime.InteropServices


type MouseWheelDirection =
    | Normal  = 0
    | Flipped = 1

module internal SDLMouseNative = 
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetMouseFocus()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern uint32 SDL_GetMouseState(int *x, int *y)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern uint32 SDL_GetGlobalMouseState(int *x, int *y)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern uint32 SDL_GetRelativeMouseState(int *x, int *y)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_WarpMouseInWindow(IntPtr window,int x, int y)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_WarpMouseGlobal(int x, int y)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetRelativeMouseMode(int enabled)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_CaptureMouse(int enabled)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetRelativeMouseMode()
