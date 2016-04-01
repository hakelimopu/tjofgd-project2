namespace SDL

open System.Runtime.InteropServices
open System

module Clipboard =
        
    module private SDLClipboardNative =
        [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
        extern int SDL_SetClipboardText(IntPtr text);
        [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
        extern IntPtr SDL_GetClipboardText();
        [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
        extern int SDL_HasClipboardText();

    let setText (text:string) :bool =
        text
        |> SDL.Utility.withUtf8String(fun ptr->SDLClipboardNative.SDL_SetClipboardText(ptr))
        <> 0

    let getText () :string =
        let ptr = SDLClipboardNative.SDL_GetClipboardText()
        let text = 
            ptr
            |> SDL.Utility.intPtrToStringUtf8
        if ptr<>IntPtr.Zero then
            SDL.Utility.SDLUtilityNative.SDL_free(ptr)
        else
            ()
        text

    let hasText () :bool =
        SDLClipboardNative.SDL_HasClipboardText()
        <> 0