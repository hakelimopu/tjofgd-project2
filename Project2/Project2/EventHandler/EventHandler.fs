module EventHandler

open GameState
open CellLocation
open MapCell
open QuitEvent
open KeyDownEvent
open IdleHandler
open Random

let handleEvent<'TRender> (renderer:GridRendererFunc<'TRender>) (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (createFunc:unit->GameState<'TRender>) (worldSize:CellLocation) (random:RandomFunc) (event:SDL.Event.Event) (state:GameState<'TRender>) : GameState<_> option =
    match event with
    | SDL.Event.Quit quitDetails   -> state |> handleQuitEvent quitDetails
    | SDL.Event.KeyDown keyDetails -> state |> handleKeyDownEvent sumLocationsFunc setVisibleFunc createFunc worldSize random keyDetails
    | _                           -> state |> onIdle renderer sumLocationsFunc


