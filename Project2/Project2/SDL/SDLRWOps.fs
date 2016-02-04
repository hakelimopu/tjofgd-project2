module SDLRWops

open System
open System.Runtime.InteropServices

#nowarn "9"

type RWops = IntPtr

module private SDLRWopsNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern RWops SDL_RWFromFile(IntPtr file, IntPtr mode)
    

