module SDLEvent

#nowarn "9"

open System.Runtime.InteropServices
open System

[<StructLayout(LayoutKind.Sequential)>]
type internal SDL_QuitEvent =
    struct
        val Type: uint32
        val Timestamp: uint32
    end

[<StructLayout(LayoutKind.Sequential)>]
type internal SDL_Keysym = 
    struct
        val Scancode: int32
        val Sym: int32
        val Mod: uint16
        val Unused: uint32
    end

[<StructLayout(LayoutKind.Sequential)>]
type internal SDL_KeyboardEvent =
    struct
        val Type: uint32
        val Timestamp: uint32
        val WindowID: uint32
        val State: uint8
        val Repeat: uint8
        val Padding2: uint8
        val Padding3: uint8
        val Keysym: SDL_Keysym
    end

type internal SDL_MouseMotionEvent =
    struct
        val Type: uint32
        val Timestamp: uint32
        val WindowID: uint32
        val Which: uint32
        val State: uint32
        val X: int32
        val Y: int32
        val Xrel: int32
        val Yrel: int32
    end

type internal SDL_MouseButtonEvent =
    struct
        val Type: uint32
        val Timestamp: uint32
        val WindowID: uint32
        val Which: uint32
        val Button: uint8
        val State: uint8
        val Clicks: uint8
        val Padding1: uint8
        val X: int32
        val Y: int32
    end


[<StructLayout(LayoutKind.Explicit, Size=56)>]
type internal SDL_Event =
    struct
        [<FieldOffset(0)>]
        val Type: uint32
        [<FieldOffset(0)>]
        val Quit: SDL_QuitEvent
        [<FieldOffset(0)>]
        val Key: SDL_KeyboardEvent
        [<FieldOffset(0)>]
        val Motion: SDL_MouseMotionEvent
        [<FieldOffset(0)>]
        val Button: SDL_MouseButtonEvent
    end

let SDL_QUIT            = 0x100u
let SDL_KEYDOWN         = 0x300u
let SDL_KEYUP           = 0x301u
let SDL_TEXTEDITING     = 0x302u
let SDL_TEXTINPUT       = 0x303u
let SDL_KEYMAPCHANGED   = 0x304u
let SDL_MOUSEMOTION     = 0x400u
let SDL_MOUSEBUTTONDOWN = 0x401u
let SDL_MOUSEBUTTONUP   = 0x402u
let SDL_MOUSEWHEEL      = 0x403u

module private SDLEventNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_WaitEvent(SDL_Event& event);

type Keysym =
    {Scancode: int32;
    Sym: int32;
    Mod: uint16}

type KeyboardEvent =
    {Timestamp: uint32;
    WindowID: uint32;
    State: uint8;
    Repeat: uint8;
    Keysym: Keysym}

type MouseMotionEvent =
    {Timestamp: uint32;
    WindowID: uint32;
    Which: uint32;
    State: uint32;
    X: int32;
    Y: int32;
    Xrel: int32;
    Yrel: int32}

type MouseButtonEvent =
    {Timestamp: uint32;
    WindowID: uint32;
    Which: uint32;
    Button: uint8;
    State: uint8;
    Clicks: uint8;
    X: int32;
    Y: int32}

type Event = 
    | KeyDown of KeyboardEvent
    | KeyUp of KeyboardEvent
    | MouseMotion of MouseMotionEvent
    | MouseButtonDown of MouseButtonEvent
    | MouseButtonUp of MouseButtonEvent
    | Other of uint32

let private toKeyboardEvent (event:SDL_KeyboardEvent) : KeyboardEvent =
    {Timestamp = event.Timestamp;
    WindowID = event.WindowID;
    State = event.State;
    Repeat = event.Repeat;
    Keysym = {Scancode = event.Keysym.Scancode; Sym=event.Keysym.Sym;Mod=event.Keysym.Mod}}

let private toMouseMotionEvent (event: SDL_MouseMotionEvent) :MouseMotionEvent =
    {Timestamp=event.Timestamp;
    WindowID=event.WindowID;
    Which=event.Which;
    State=event.State;
    X=event.X;
    Y=event.Y;
    Xrel=event.Xrel;
    Yrel=event.Yrel}

let private toMouseButtonEvent (event:SDL_MouseButtonEvent) :MouseButtonEvent =
    {Timestamp = event.Timestamp;
    WindowID = event.WindowID;
    Which = event.Which;
    Button = event.Button;
    State = event.State;
    Clicks = event.Clicks;
    X = event.X;
    Y = event.Y}
    
let waitEvent () =
    let mutable event = new SDL_Event()
    let result = SDLEventNative.SDL_WaitEvent(&event)
    if result = 1 then
        if event.Type = SDL_KEYDOWN then
            event.Key |> toKeyboardEvent |> KeyDown |> Some

        elif event.Type = SDL_KEYUP then
            event.Key |> toKeyboardEvent |> KeyUp |> Some

        elif event.Type = SDL_MOUSEMOTION then
            event.Motion |> toMouseMotionEvent |> MouseMotion |> Some

        elif event.Type = SDL_MOUSEBUTTONDOWN then
            event.Button |> toMouseButtonEvent |> MouseButtonDown |> Some

        elif event.Type = SDL_MOUSEBUTTONUP then
            event.Button |> toMouseButtonEvent |> MouseButtonUp |> Some

        else
            event.Type |> Other |> Some
    else
        None