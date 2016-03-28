module IdleHandler

open GameState
open CellLocation

type GridRendererFunc<'TRender> = SumLocationsFunc -> PlayState<'TRender> -> PlayState<'TRender>
type PlayStateAdapterFunc<'TRender> = PlayState<'TRender> -> GameState<'TRender>

let private onIdleWithPlayState<'TRender> (renderer:GridRendererFunc<'TRender>) (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (adapter:PlayStateAdapterFunc<'TRender>) (state:PlayState<'TRender>) :GameState<'TRender> option =
    state
    |> renderer sumLocationsFunc 
    |> adapter
    |> Some

let internal onIdle<'TRender> (renderer:GridRendererFunc<'TRender>) (sumLocationsFunc:CellLocation->CellLocation->CellLocation) (state:GameState<'TRender>): GameState<'TRender> option =
    match state with
    | PlayState x -> 
        x |> onIdleWithPlayState<'TRender> renderer sumLocationsFunc PlayState
    | DeadState x -> 
        x |> onIdleWithPlayState<'TRender> renderer sumLocationsFunc DeadState


