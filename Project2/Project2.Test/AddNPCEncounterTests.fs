[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
module AddNPCEncounterTests

open Xunit
open ActorUpdate
open GameState
open CellLocation

[<Fact>]
let ``addNPCEncounter with initial play state having no encounters`` () =
    let encounterDetail = 
        {Location={Column=0<cell>;Row=0<cell>};
         Type=RanIntoStorm;
         Title="";
         Message=[];
         Choices=[];
         CurrentChoice=0}

    let initialState =
        {RenderGrid=Map.empty;
         Encounters=None;
         Actors=Map.empty;
         MapGrid=Map.empty}

    let expected =
        {initialState with Encounters = [encounterDetail] |> NPCEncounters |> Some}

    let actual =
        (encounterDetail, initialState)
        ||> addNPCEncounter

    Assert.Equal(expected, actual)

[<Fact>]
let ``addNPCEncounter with initial play state with npc encounter`` () =
    let initialEncounter = 
        {Location={Column=1<cell>;Row= -1<cell>};
         Type=DockedWithIsland;
         Title="";
         Message=[];
         Choices=[];
         CurrentChoice=0}

    let encounterDetail = 
        {Location={Column=0<cell>;Row=0<cell>};
         Type=RanIntoStorm;
         Title="";
         Message=[];
         Choices=[];
         CurrentChoice=0}

    let initialState =
        {RenderGrid=Map.empty;
         Encounters=[initialEncounter] |> NPCEncounters |> Some;
         Actors=Map.empty;
         MapGrid=Map.empty}

    let expected =
        {initialState with Encounters = [initialEncounter; encounterDetail] |> NPCEncounters |> Some}

    let actual =
        (encounterDetail, initialState)
        ||> addNPCEncounter

    Assert.Equal(expected, actual)

[<Fact>]
let ``addNPCEncounter with initial play state with pc encounter`` () =
    let initialEncounter = 
        {Location={Column=1<cell>;Row= -1<cell>};
         Type=DockedWithIsland;
         Title="";
         Message=[];
         Choices=[];
         CurrentChoice=0}

    let encounterDetail = 
        {Location={Column=0<cell>;Row=0<cell>};
         Type=RanIntoStorm;
         Title="";
         Message=[];
         Choices=[];
         CurrentChoice=0}

    let initialState =
        {RenderGrid=Map.empty;
         Encounters=initialEncounter |> PCEncounter |> Some;
         Actors=Map.empty;
         MapGrid=Map.empty}

    Assert.Throws<IncompatibleEncounterType>(fun () -> (encounterDetail, initialState) ||> addNPCEncounter |> ignore)
