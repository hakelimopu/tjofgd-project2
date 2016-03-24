[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
module UpdateActorTests

open Xunit
open ActorUpdate
open GameState
open CellLocation
open MapObject
open Random

//let updateActor (sumLocationsFunc:SumLocationsFunc) (random:RandomFunc) (actorLocation:CellLocation) (actor:MapObject) (currentTurn:float<turn>) (playState:PlayState, flag:bool) :PlayState * bool=
exception private InvalidCallToRandomFunc

[<Fact>]
let ``updateActor with no updates and false initial flag`` () =
    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    let sumLocationsFunc = sumLocationsWrapped worldSize
    let randomFunc parm =
        raise InvalidCallToRandomFunc
    let initialActorLocation = {Column = 0<cell>;Row = 1<cell>}
    let initialActor = {CurrentTurn = 0.0<turn>; Detail = {Damage = 1<health>} |> Storm}
    let initialCurrentTurn = 0.0<turn>
    let initialPlayState = 
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty;
         MapGrid=Map.empty}
    let initialFlag = false

    let expectedFlag = initialFlag
    let expectedPlayState = initialPlayState

    let actualPlayState, actualFlag = updateActor sumLocationsFunc randomFunc initialCurrentTurn (initialPlayState, initialFlag) initialActorLocation initialActor

    Assert.Equal(expectedPlayState, actualPlayState)
    Assert.Equal(expectedFlag, actualFlag)

[<Fact>]
let ``updateActor with no updates and true initial flag`` () =
    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    let sumLocationsFunc = sumLocationsWrapped worldSize
    let randomFunc parm =
        raise InvalidCallToRandomFunc
    let initialActorLocation = {Column = 0<cell>;Row = 1<cell>}
    let initialActor = {CurrentTurn = 0.0<turn>; Detail = {Damage = 1<health>} |> Storm}
    let initialCurrentTurn = 0.0<turn>
    let initialPlayState = 
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty;
         MapGrid=Map.empty}
    let initialFlag = true

    let expectedFlag = initialFlag
    let expectedPlayState = initialPlayState

    let actualPlayState, actualFlag = updateActor sumLocationsFunc randomFunc initialCurrentTurn (initialPlayState, initialFlag) initialActorLocation initialActor

    Assert.Equal(expectedPlayState, actualPlayState)
    Assert.Equal(expectedFlag, actualFlag)

[<Fact>]
let ``updateActor update storm`` () =
    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    let sumLocationsFunc = sumLocationsWrapped worldSize
    let randomFuncCounter = ref 0
    let randomFunc (counter:ref<int>) parm =
        counter := !counter + 1
        match !counter with
        | 1 ->  1 |> Int
        | 2 -> -1 |> Int
        | _ -> raise InvalidCallToRandomFunc
    let initialActorLocation = {Column = 0<cell>;Row = 1<cell>}
    let initialActor = {CurrentTurn = 0.0<turn>; Detail = {Damage = 1<health>} |> Storm}
    let initialCurrentTurn = 1.0<turn>
    let initialPlayState = 
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty;
         MapGrid=Map.empty}
    let initialFlag = false

    let expectedActor = {CurrentTurn = 0.5<turn>; Detail = {Damage = 1<health>} |> Storm}
    let expectedLocation = {Column = 1<cell>; Row = 0<cell>}
    let expectedFlag = true
    let expectedPlayState = {initialPlayState with Actors = initialPlayState.Actors |> Map.add expectedLocation expectedActor}

    let actualPlayState, actualFlag = updateActor sumLocationsFunc (randomFunc randomFuncCounter) initialCurrentTurn (initialPlayState, initialFlag) initialActorLocation initialActor

    Assert.Equal(expectedPlayState, actualPlayState)
    Assert.Equal(expectedFlag, actualFlag)

