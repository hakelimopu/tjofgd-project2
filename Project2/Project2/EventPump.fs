module EventPump

let rec eventPump (renderHandler:'TState->unit) (eventHandler:SDLEvent.Event->'TState->'TState option) (state:'TState) : unit =
    match SDLEvent.pollEvent() with
    | Some event ->
        match state |> eventHandler event with
        | Some newState -> 
            eventPump renderHandler eventHandler newState
        | None -> ()
    | None -> 
        state
        |> renderHandler
        eventPump renderHandler eventHandler state


