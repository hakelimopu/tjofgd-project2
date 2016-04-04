[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
module UpdateStormActorTests

open Xunit
open GameState
open CellLocation
open MapObject
open Random
open StormUpdate

exception private InvalidCallToRandomFunc

[<Fact>]
let ``updateStormActor with currentTurn equal to stormTurn`` () =
    let randomFunc parm =
        raise InvalidCallToRandomFunc

    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    
    let initialState =
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty;
         MapGrid=Map.empty}

    let stormLocation = {Column = 0<cell>;Row = 1<cell>}

    let stormProperties = {Damage = 1<health>}

    let stormTurn = 1.0<turn>

    let currentTurn = 1.0<turn>

    let expected = initialState

    let actual =
        initialState 
        |> updateStormActor (sumLocationsWrapped worldSize) randomFunc stormLocation stormProperties stormTurn currentTurn

    Assert.Equal(expected, actual)

[<Fact>]
let ``updateStormActor with currentTurn greater than stormTurn and no interaction`` () =
    let randomFuncCounter = ref 0
    let randomFunc (counter:ref<int>) parm =
        counter := !counter + 1
        match !counter with
        | 1 -> -1 |> Int
        | 2 ->  1 |> Int
        | _ -> raise InvalidCallToRandomFunc

    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    
    let stormLocation = {Column = 0<cell>;Row = 1<cell>}

    let stormProperties = {Damage = 1<health>}

    let stormTurn = 1.0<turn>

    let actor = {CurrentTurn = stormTurn; Detail = stormProperties |> Storm}

    let initialState =
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add stormLocation actor;
         MapGrid=Map.empty}

    let currentTurn = 2.0<turn>

    let expectedStormTurn = 1.5<turn>

    let expectedStormLocation = {Column = 9<cell>;Row = 2<cell>}

    let expectedActor = {CurrentTurn = expectedStormTurn; Detail = stormProperties |> Storm}

    let expected = 
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add expectedStormLocation expectedActor;
         MapGrid=Map.empty}

    let actual =
        initialState 
        |> updateStormActor (sumLocationsWrapped worldSize) (randomFunc randomFuncCounter) stormLocation stormProperties stormTurn currentTurn

    Assert.Equal(expected, actual)

[<Fact>]
let ``updateStormActor combine storms`` () =
    let randomFuncCounter = ref 0
    let randomFunc (counter:ref<int>) parm =
        counter := !counter + 1
        match !counter with
        | 1 -> -1 |> Int
        | 2 ->  1 |> Int
        | _ -> raise InvalidCallToRandomFunc

    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    
    let stormLocation = {Column = 0<cell>;Row = 1<cell>}

    let stormProperties = {Damage = 1<health>}

    let stormTurn = 1.0<turn>

    let otherStormLocation = {Column = 9<cell>;Row = 2<cell>}

    let otherStormProperties = {Damage = 2<health>}

    let otherStormTurn = 1.0<turn>

    let actor = {CurrentTurn = stormTurn; Detail = stormProperties |> Storm}

    let otherActor = {CurrentTurn = otherStormTurn; Detail = otherStormProperties |> Storm}

    let initialState =
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add stormLocation actor |> Map.add otherStormLocation otherActor;
         MapGrid=Map.empty}

    let currentTurn = 2.0<turn>

    let expectedStormTurn = 1.25<turn>

    let expectedStormLocation = {Column = 9<cell>;Row = 2<cell>}

    let expectedStormProperties = {Damage = 3<health>}

    let expectedActor = {CurrentTurn = expectedStormTurn; Detail = expectedStormProperties |> Storm}

    let expected = 
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add expectedStormLocation expectedActor;
         MapGrid=Map.empty}

    let actual =
        initialState 
        |> updateStormActor (sumLocationsWrapped worldSize) (randomFunc randomFuncCounter) stormLocation stormProperties stormTurn currentTurn

    Assert.Equal(expected, actual)

[<Fact>]
let ``updateStormActor strike pirate`` () =
    let randomFuncCounter = ref 0
    let randomFunc (counter:ref<int>) parm =
        counter := !counter + 1
        match !counter with
        | 1 -> -1 |> Int
        | 2 ->  1 |> Int
        | _ -> raise InvalidCallToRandomFunc

    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    
    let stormLocation = {Column = 0<cell>;Row = 1<cell>}

    let stormProperties = {Damage = 1<health>}

    let stormTurn = 1.0<turn>

    let otherActorLocation = {Column = 9<cell>;Row = 2<cell>}

    let otherActorProperties = {Hull = 10<health>; Attitude = PirateAttitude.Enemy}

    let otherStormTurn = 1.0<turn>

    let actor = {CurrentTurn = stormTurn; Detail = stormProperties |> Storm}

    let otherActor = {CurrentTurn = otherStormTurn; Detail = otherActorProperties |> Pirate}

    let initialState =
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add stormLocation actor |> Map.add otherActorLocation otherActor;
         MapGrid=Map.empty}

    let currentTurn = 2.0<turn>

    let expectedActorTurn = 1.0<turn>

    let expectedActorLocation = {Column = 9<cell>;Row = 2<cell>}

    let expectedActorProperties = {Hull = 9<health>; Attitude = PirateAttitude.Enemy}

    let expectedActor = {CurrentTurn = expectedActorTurn; Detail = expectedActorProperties |> Pirate}

    let expected = 
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add expectedActorLocation expectedActor;
         MapGrid=Map.empty}

    let actual =
        initialState 
        |> updateStormActor (sumLocationsWrapped worldSize) (randomFunc randomFuncCounter) stormLocation stormProperties stormTurn currentTurn

    Assert.Equal(expected, actual)

[<Fact>]
let ``updateStormActor strike and eliminate pirate`` () =
    let randomFuncCounter = ref 0
    let randomFunc (counter:ref<int>) parm =
        counter := !counter + 1
        match !counter with
        | 1 -> -1 |> Int
        | 2 ->  1 |> Int
        | _ -> raise InvalidCallToRandomFunc

    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    
    let stormLocation = {Column = 0<cell>;Row = 1<cell>}

    let stormProperties = {Damage = 1<health>}

    let stormTurn = 1.0<turn>

    let otherActorLocation = {Column = 9<cell>;Row = 2<cell>}

    let otherActorProperties = {Hull = 1<health>; Attitude = PirateAttitude.Enemy}

    let otherStormTurn = 1.0<turn>

    let actor = {CurrentTurn = stormTurn; Detail = stormProperties |> Storm}

    let otherActor = {CurrentTurn = otherStormTurn; Detail = otherActorProperties |> Pirate}

    let initialState =
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add stormLocation actor |> Map.add otherActorLocation otherActor;
         MapGrid=Map.empty}

    let currentTurn = 2.0<turn>

    let expected = 
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty;
         MapGrid=Map.empty}

    let actual =
        initialState 
        |> updateStormActor (sumLocationsWrapped worldSize) (randomFunc randomFuncCounter) stormLocation stormProperties stormTurn currentTurn

    Assert.Equal(expected, actual)

[<Fact>]
let ``updateStormActor strike boat`` () =
    let randomFuncCounter = ref 0
    let randomFunc (counter:ref<int>) parm =
        counter := !counter + 1
        match !counter with
        | 1 -> -1 |> Int
        | 2 ->  1 |> Int
        | _ -> raise InvalidCallToRandomFunc

    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    
    let stormLocation = {Column = 0<cell>;Row = 1<cell>}

    let stormProperties = {Damage = 1<health>}

    let stormTurn = 1.0<turn>

    let otherActorLocation = {Column = 9<cell>;Row = 2<cell>}

    let otherActorProperties = 
        {Hull = 10<health>;
         MaximumHull = 10<health>;
         Wallet = 0.0<currency>;
         Quest = None;
         GenerateNextStorm = 5.0<turn>;
         EquipmentCapacity=1<slot>;
         Equipment=Seq.empty}

    let otherStormTurn = 1.0<turn>

    let actor = {CurrentTurn = stormTurn; Detail = stormProperties |> Storm}

    let otherActor = {CurrentTurn = otherStormTurn; Detail = otherActorProperties |> Boat}

    let initialState =
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add stormLocation actor |> Map.add otherActorLocation otherActor;
         MapGrid=Map.empty}

    let currentTurn = 2.0<turn>

    let expectedActorLocation = stormLocation

    let expectedActorProperties = stormProperties

    let expectedActor = {CurrentTurn = currentTurn; Detail = expectedActorProperties |> Storm}

    let expectedEncounters =
        [{Location=stormLocation;
         Title="Storm!";
         Type=RanIntoStorm;
         Message=["You have run into a storm;";"it has damaged your boat!"];
         Choices=[{Text="OK";Response=Confirm}];
         CurrentChoice=0}]
        |> NPCEncounters
        |> Some

    let expected = 
        {RenderData=Map.empty;
         Encounters=expectedEncounters;
         Actors=Map.empty |> Map.add expectedActorLocation expectedActor |> Map.add otherActorLocation otherActor;
         MapGrid=Map.empty}

    let actual =
        initialState 
        |> updateStormActor (sumLocationsWrapped worldSize) (randomFunc randomFuncCounter) stormLocation stormProperties stormTurn currentTurn

    Assert.Equal(expected, actual)

[<Fact>]
let ``updateStormActor strike sea monster`` () =
    let randomFuncCounter = ref 0
    let randomFunc (counter:ref<int>) parm =
        counter := !counter + 1
        match !counter with
        | 1 -> -1 |> Int
        | 2 ->  1 |> Int
        | _ -> raise InvalidCallToRandomFunc

    let worldSize = {Column = 10<cell>;Row = 11<cell>}
    
    let stormLocation = {Column = 0<cell>;Row = 1<cell>}

    let stormProperties = {Damage = 1<health>}

    let stormTurn = 1.0<turn>

    let otherActorLocation = {Column = 9<cell>;Row = 2<cell>}

    let otherActorProperties = 
        {Health = 10<health>;
         Attitude = SeaMonsterAttitude.Enraged}

    let otherStormTurn = 1.0<turn>

    let actor = {CurrentTurn = stormTurn; Detail = stormProperties |> Storm}

    let otherActor = {CurrentTurn = otherStormTurn; Detail = otherActorProperties |> SeaMonster}

    let initialState =
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add stormLocation actor |> Map.add otherActorLocation otherActor;
         MapGrid=Map.empty}

    let currentTurn = 2.0<turn>

    let expected = 
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty |> Map.add otherActorLocation otherActor;
         MapGrid=Map.empty}

    let actual =
        initialState 
        |> updateStormActor (sumLocationsWrapped worldSize) (randomFunc randomFuncCounter) stormLocation stormProperties stormTurn currentTurn

    Assert.Equal(expected, actual)
