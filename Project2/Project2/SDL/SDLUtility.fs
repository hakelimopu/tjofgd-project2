module SDLUtility

#nowarn "9"

open System.Text
open System.Runtime.InteropServices
open System
open Microsoft.FSharp.NativeInterop

[<Measure>] type px
[<Measure>] type bit
[<Measure>] type bytes

let internal withUtf8String (func: IntPtr->'T) (text:string) =
    let bytes = Encoding.UTF8.GetBytes(text)
    let pinnedArray = GCHandle.Alloc(bytes, GCHandleType.Pinned)
    let result = pinnedArray.AddrOfPinnedObject() |> func
    pinnedArray.Free()
    result


let internal intPtrToStringUtf8 (ptr:IntPtr): string =
    if ptr = IntPtr.Zero then
        null
    else
        let mutable bytePtr = 
            ptr
            |> NativePtr.ofNativeInt<byte>
        let mutable byteSequence = Seq.empty<byte>
        while (bytePtr |> NativePtr.read) <> 0uy do
            byteSequence <- [bytePtr |> NativePtr.read] |> Seq.append byteSequence
            bytePtr <- 1 |> NativePtr.add bytePtr
        Encoding.UTF8.GetString(byteSequence |> Seq.toArray)
