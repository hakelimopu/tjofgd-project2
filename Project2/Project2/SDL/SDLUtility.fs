module SDLUtility

open System.Text
open System.Runtime.InteropServices
open System

let withUtf8String (func: IntPtr->'T) (text:string) =
    let bytes = Encoding.UTF8.GetBytes(text)
    let pinnedArray = GCHandle.Alloc(bytes, GCHandleType.Pinned)
    let result = pinnedArray.AddrOfPinnedObject() |> func
    pinnedArray.Free()
    result


