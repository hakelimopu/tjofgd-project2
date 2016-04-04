[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
module AddNPCStormEncounterTests

open Xunit
open ActorUpdate
open GameState
open CellLocation
open NPCEncounterUtilities

[<Fact>]
let ``addNPCStormEncounter with specified location`` () =
    let location = {Column=0<cell>;Row=1<cell>}

    let expectedEncounter = 
        {Location=location;
         Title="Storm!";
         Type=RanIntoStorm;
         Message=["You have run into a storm;";"it has damaged your boat!"];
         Choices=[{Text="OK";Response=Common Confirm}];
         CurrentChoice=0}

    let initialState =
        {RenderData=Map.empty;
         Encounters=None;
         Actors=Map.empty;
         MapGrid=Map.empty}

    let expected =
        {initialState with Encounters = [expectedEncounter] |> NPCEncounters |> Some}

    let actual = 
        (location, initialState)
        ||> addNPCStormEncounter

    Assert.Equal(expected, actual)