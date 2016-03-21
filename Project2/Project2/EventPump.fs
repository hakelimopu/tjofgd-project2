module EventPump

//The event pump... the heart of every SDL based game
let rec eventPump (eventPoller:unit->SDLEvent.Event option) (idleHandler:'TState->unit) (eventHandler:SDLEvent.Event->'TState->'TState option) (state:'TState) : unit =

    //poll for events
    match eventPoller() with

    //there is an event!
    | Some event ->
        
        //see how the state gets mutated by the event handler...
        match state |> eventHandler event with
        //it results in a new state, so keep pumping!
        | Some newState -> eventPump eventPoller idleHandler eventHandler newState
        //there is no longer a state, so exit event pump!
        | None -> ()

    //there are no events!
    | None -> 
        //no events, so do some idling!
        state
        |> idleHandler

        //keep pumping with the original state!
        eventPump eventPoller idleHandler eventHandler state


