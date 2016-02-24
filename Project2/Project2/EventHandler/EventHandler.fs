﻿module EventHandler

open GameState
open CellLocation
open MapCell
open QuitEvent
open KeyDownEvent
open IdleHandler

let handleEvent (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (setVisibleFunc:CellLocation->CellMap<MapCell>->CellMap<MapCell>) (event:SDLEvent.Event) (state:GameState) : GameState option =
    match event with
    | SDLEvent.Quit quitDetails -> state |> handleQuitEvent quitDetails
    | SDLEvent.KeyDown keyDetails -> state |> handleKeyDownEvent sumLocationsFunc setVisibleFunc keyDetails
    | _ -> state |> onIdle sumLocationsFunc


