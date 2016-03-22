[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute>]
module SDLUtility

#nowarn "9"

open System.Text
open System.Runtime.InteropServices
open System
open Microsoft.FSharp.NativeInterop

[<Measure>] type px
[<Measure>] type bit
[<Measure>] type bytes
[<Measure>] type ms

let private withString (encoder:string->byte[]) (func:IntPtr->'T) (text:string) =
    let bytes = encoder(text)
    let pinnedArray = GCHandle.Alloc(bytes, GCHandleType.Pinned)
    let result = pinnedArray.AddrOfPinnedObject() |> func
    pinnedArray.Free()
    result

let internal withUtf8String (func:IntPtr->'T) (text:string) =
    withString Encoding.UTF8.GetBytes func text

let internal withAsciiString (func: IntPtr->'T) (text:string) =
    withString Encoding.ASCII.GetBytes func text


let private intPtrToString (encoder:byte[]->string) (ptr:IntPtr):string = 
    if ptr = IntPtr.Zero then
        null
    else
        let byteEmitter (bytePtr:nativeptr<byte>) =
            match bytePtr |> NativePtr.read with
            | 0uy ->  None
            | nextByte -> Some (nextByte, 1 |> NativePtr.add bytePtr)

        ptr 
        |> NativePtr.ofNativeInt<byte>
        |> Seq.unfold byteEmitter
        |> Seq.toArray
        |> encoder


let internal intPtrToStringUtf8 =
    intPtrToString Encoding.UTF8.GetString


let internal intPtrToStringAscii=
    intPtrToString Encoding.ASCII.GetString

type Pointer(ptr:IntPtr, destroyFunc: IntPtr->unit) =
    let mutable windowPointer = ptr
    member this.Pointer
        with get() = windowPointer
    member this.Destroy() =
        if ptr = IntPtr.Zero then
            ()
        else
            destroyFunc(ptr)
            windowPointer <- IntPtr.Zero
    interface IDisposable with
        member this.Dispose()=
            this.Destroy()
