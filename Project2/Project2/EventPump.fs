module EventPump

//The event pump... the heart of every SDL based game
let rec eventPump (idleHandler:'TState->unit) (eventHandler:SDLEvent.Event->'TState->'TState option) (state:'TState) : unit =

    //poll for events
    match SDLEvent.pollEvent() with

    //there is an event!
    | Some event ->
        
        //see how the state gets mutated by the event handler...
        match state |> eventHandler event with
        //it results in a new state, so keep pumping!
        | Some newState -> eventPump idleHandler eventHandler newState
        //there is no longer a state, so exit event pump!
        | None -> ()

    //there are no events!
    | None -> 
        //no events, so do some idling!
        state
        |> idleHandler

        //keep pumping with the original state!
        eventPump idleHandler eventHandler state


