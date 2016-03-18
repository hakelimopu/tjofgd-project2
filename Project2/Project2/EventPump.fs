module EventPump

//The event pump... the heart of every SDL based game
let rec eventPump (idleHandler:'TState->unit) (eventHandler:SDLEvent.Event->'TState->'TState option) (state:'TState) : unit =

    //for convenience, when we call ourselves later, we only change the value of state
    let keepPumping = eventPump idleHandler eventHandler

    //poll for events
    match SDLEvent.pollEvent() with

    //there is an event!
    | Some event ->
        
        //see how the state gets mutated by the event handler...
        match state |> eventHandler event with
        //it results in a new state, so keep pumping!
        | Some newState -> keepPumping newState
        //there is no longer a state, so exit event pump!
        | None -> ()

    //there are no events!
    | None -> 
        //no events, so do some idling!
        state
        |> idleHandler

        //keep pumping with the original state!
        keepPumping state


