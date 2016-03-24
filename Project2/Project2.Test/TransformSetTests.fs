module TransformSetTests

open CellLocation
open Xunit

[<Fact>]
let ``transformSet empty set`` () =
    let worldSize  = (10<cell>, 11<cell>) ||> make

    let sumLocationsFunc = sumLocationsWrapped worldSize

    let locations = Set.empty<CellLocation>

    let delta = (1<cell>, -1<cell>) ||> make

    let expected = Set.empty<CellLocation>

    let actual = 
        transformSet sumLocationsFunc locations delta

    Assert.False(expected <> actual)

[<Fact>]
let ``transformSet one location`` () =
    let worldSize  = (10<cell>, 11<cell>) ||> make

    let sumLocationsFunc = sumLocationsWrapped worldSize

    let locations = 
        [(0<cell>,0<cell>)]
        |> Seq.map (fun e-> e ||> CellLocation.make)
        |> Set.ofSeq

    let delta = (1<cell>, -1<cell>) ||> make

    let expected =
        [(1<cell>, 10<cell>)]
        |> Seq.map (fun e-> e ||> CellLocation.make)
        |> Set.ofSeq

    let actual = 
        transformSet sumLocationsFunc locations delta

    Assert.False(expected <> actual)
