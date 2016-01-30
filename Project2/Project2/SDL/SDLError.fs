module SDLError

#nowarn "9"

open System.Runtime.InteropServices
open System
open Microsoft.FSharp.NativeInterop
open System.Text
open SDLUtility

module private SDLErrorNative = 
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern void SDL_ClearError()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern IntPtr SDL_GetError()
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)>]
    extern int SDL_SetError(IntPtr fmt)

let private intPtrToStringUtf8 (ptr:IntPtr): string =
    if ptr = IntPtr.Zero then
        null
    else
        let mutable bytePtr = 
            ptr
            |> nativeint
            |> NativePtr.ofNativeInt<byte>
        let mutable byteSequence = Seq.empty<byte>
        while (bytePtr |> NativePtr.read) <> 0uy do
            byteSequence <- [bytePtr |> NativePtr.read] |> Seq.append byteSequence
            bytePtr <- 1 |> NativePtr.add bytePtr
        Encoding.UTF8.GetString(byteSequence |> Seq.toArray)

let set (errorString:string) =
    errorString
    |> SDLUtility.withUtf8String (fun ptr -> SDLErrorNative.SDL_SetError(ptr) |> ignore)

let get () =
    SDLErrorNative.SDL_GetError()
    |> intPtrToStringUtf8

let clear () =
    SDLErrorNative.SDL_ClearError()