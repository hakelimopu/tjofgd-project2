﻿module EventHandler

open GameState
open CellLocation
open MapCell
open QuitEvent
open KeyDownEvent
open IdleHandler

let handleEvent (sumLocationsFunc:SumLocationsFunc) (setVisibleFunc:SetVisibleFunc) (createFunc:unit->GameState) (worldSize:CellLocation) (random:System.Random) (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit quitDetails   -> state |> handleQuitEvent quitDetails
    | SDLEvent.KeyDown keyDetails -> state |> handleKeyDownEvent sumLocationsFunc setVisibleFunc createFunc worldSize random keyDetails
    | _                           -> state |> onIdle sumLocationsFunc

