module SDLRWops

open System
open System.Runtime.InteropServices

#nowarn "9"

type RWops = IntPtr

module internal SDLRWopsNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)>]
    extern RWops SDL_RWFromFile(IntPtr file, string mode)
    

