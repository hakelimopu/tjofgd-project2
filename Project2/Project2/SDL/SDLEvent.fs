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

type EventType =
    | Quit                     = 0x100
    | AppTerminating           = 0x101
    | AppLowmemory             = 0x102
    | AppWillEnterBackground   = 0x103
    | AppDidEnterBackground    = 0x104
    | AppWillEnterForeground   = 0x105
    | AppDidEnterForeground    = 0x106
    | WindowEvent              = 0x200
    | SysWMEvent               = 0x201
    | KeyDown                  = 0x300
    | KeyUp                    = 0x301
    | TextEditing              = 0x302
    | TextInput                = 0x303
    | KeyMapChanged            = 0x304
    | MouseMotion              = 0x400
    | MouseButtonDown          = 0x401
    | MouseButtonUp            = 0x402
    | MouseWheel               = 0x403
    | JoyAxisMotion            = 0x600
    | JoyBallMotion            = 0x601
    | JoyHatMotion             = 0x602
    | JoyButtonDown            = 0x603
    | JoyButtonUp              = 0x604
    | JoyDeviceAdded           = 0x605
    | JoyDeviceRemoved         = 0x606
    | ControllerAxisMotion     = 0x650 
    | ControllerButtonDown     = 0x651   
    | ControllerButtonUp       = 0x652
    | ControllerDeviceAdded    = 0x653
    | ControllerDeviceRemoved  = 0x654
    | ControllerDeviceRemapped = 0x655
    | FingerDown               = 0x700
    | FingerUp                 = 0x701
    | FingerMotion             = 0x702
    | DollarGesture            = 0x800
    | DollarRecord             = 0x801
    | MultiGesture             = 0x802
    | ClipboardUpdate          = 0x900 
    | DropFile                 = 0x1000
    | AudioDeviceAdded         = 0x1100
    | AudioDeviceRemoved       = 0x1101      
    | RenderTargetsReset       = 0x2000
    | RenderDeviceReset        = 0x2001
    | UserEvent                = 0x8000
    | LastEvent                = 0xFFFF

module private SDLEventNative =
    [<DllImport(@"SDL2.dll", CallingConvention = CallingConvention.Cdecl)>]
    extern int SDL_WaitEvent(SDL_Event& event);

type QuitEvent =
    {Timestamp:uint32}

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
    | Quit of QuitEvent
    | KeyDown of KeyboardEvent
    | KeyUp of KeyboardEvent
    | MouseMotion of MouseMotionEvent
    | MouseButtonDown of MouseButtonEvent
    | MouseButtonUp of MouseButtonEvent
    | Other of uint32

let private toQuitEvent (event:SDL_QuitEvent) :QuitEvent =
    {Timestamp = event.Timestamp}

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
    let result = SDLEventNative.SDL_WaitEvent(&event) = 1
    match result, (event.Type |> int |> enum<EventType>) with
    | true, EventType.Quit -> event.Quit |> toQuitEvent |> Quit |> Some
    | true, EventType.KeyDown -> event.Key |> toKeyboardEvent |> KeyDown |> Some
    | true, EventType.KeyUp -> event.Key |> toKeyboardEvent |> KeyUp |> Some
    | true, EventType.MouseMotion -> event.Motion |> toMouseMotionEvent |> MouseMotion |> Some
    | true, EventType.MouseButtonDown -> event.Button |> toMouseButtonEvent |> MouseButtonDown |> Some
    | true, EventType.MouseButtonUp -> event.Button |> toMouseButtonEvent |> MouseButtonUp |> Some
    | true, _ -> event.Type |> Other |> Some
    | _, _ -> None

