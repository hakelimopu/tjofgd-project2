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

type WindowEvent =
    | None        = 0
    | Shown       = 1
    | Hidden      = 2
    | Exposed     = 3
    | Moved       = 4
    | Resized     = 5
    | SizeChanged = 6
    | Minimized   = 7
    | Maximized   = 8
    | Restored    = 9
    | Enter       = 10
    | Leave       = 11
    | FocusGained = 12
    | FocusLost   = 13
    | Close       = 14

type Window = SDLUtility.Pointer

module private SDLWindowNative =
    //create and destroy
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateWindow(IntPtr title, int x, int y, int w, int h, uint32 flags)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_CreateWindowFrom(IntPtr data)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DestroyWindow(IntPtr window)

    //video drivers
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetNumVideoDrivers()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetVideoDriver(int index)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetCurrentVideoDriver()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_VideoInit(IntPtr driver_name)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_VideoQuit()

    //displays
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetNumVideoDisplays()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetDisplayName(int displayIndex)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetDisplayBounds(int displayIndex, IntPtr rect)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetDisplayDPI(int displayIndex, IntPtr ddpi, IntPtr hdpi, IntPtr vdpi)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]

    //display modes
    extern int SDL_GetNumDisplayModes(int displayIndex)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetDisplayMode(int displayIndex, int modeIndex,IntPtr mode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetDesktopDisplayMode(int displayIndex, IntPtr mode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetCurrentDisplayMode(int displayIndex, IntPtr mode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetClosestDisplayMode(int displayIndex, IntPtr mode, IntPtr closest)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetWindowDisplayIndex(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetWindowDisplayMode(IntPtr window, IntPtr mode)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetWindowDisplayMode(IntPtr window,IntPtr mode)

    //window properties
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern uint32 SDL_GetWindowPixelFormat(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern uint32 SDL_GetWindowID(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern uint32 SDL_GetWindowFlags(IntPtr window)

    //title
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_SetWindowTitle(IntPtr window, IntPtr title)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetWindowTitle(IntPtr window)

    //icon
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_SetWindowIcon(IntPtr window,IntPtr icon)

    //data
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_SetWindowData(IntPtr window, IntPtr name,IntPtr userdata)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetWindowData(IntPtr window, IntPtr name)

    //position
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_SetWindowPosition(IntPtr window,int x, int y)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_GetWindowPosition(IntPtr window,IntPtr x, IntPtr y)

    //size
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_SetWindowSize(IntPtr window, int w,int h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_GetWindowSize(IntPtr window, IntPtr w,IntPtr h)

    //minimum size
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_SetWindowMinimumSize(IntPtr window,int min_w, int min_h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_GetWindowMinimumSize(IntPtr window,IntPtr w, IntPtr h)

    //maximum size
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_SetWindowMaximumSize(IntPtr window,int max_w, int max_h)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_GetWindowMaximumSize(IntPtr window,IntPtr w, IntPtr h)

    //bordered
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_SetWindowBordered(IntPtr window,int bordered)

    //show/hide
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_ShowWindow(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_HideWindow(IntPtr window)

    //window state
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RaiseWindow(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_MaximizeWindow(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_MinimizeWindow(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_RestoreWindow(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetWindowFullscreen(IntPtr window,uint32 flags)

    //window surface
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetWindowSurface(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpdateWindowSurface(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_UpdateWindowSurfaceRects(IntPtr window, IntPtr rects,int numrects)

    //grab
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_SetWindowGrab(IntPtr window,int grabbed)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetWindowGrab(IntPtr window)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetGrabbedWindow()

    //brightness
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetWindowBrightness(IntPtr window, float brightness)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern float SDL_GetWindowBrightness(IntPtr window)

    //gamma ramp
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetWindowGammaRamp(IntPtr window, IntPtr red, IntPtr green, IntPtr blue)
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_GetWindowGammaRamp(IntPtr window,IntPtr red,IntPtr green,IntPtr blue)

    //hit test
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_SetWindowHitTest(IntPtr window,IntPtr callback,IntPtr callback_data)

    //screen saver
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_IsScreenSaverEnabled()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_EnableScreenSaver()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_DisableScreenSaver()

    //window from id
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetWindowFromID(uint32 id)

let create (title:string) (x:int<px>) (y:int<px>) (w:int<px>) (h:int<px>) (flags:uint32) :Window =
    let ptr = 
        title
        |> SDLUtility.withUtf8String (fun ptr -> SDLWindowNative.SDL_CreateWindow(ptr, x /1<px>, y /1<px>, w /1<px>, h /1<px>, flags))
    new SDLUtility.Pointer(ptr, SDLWindowNative.SDL_DestroyWindow)
