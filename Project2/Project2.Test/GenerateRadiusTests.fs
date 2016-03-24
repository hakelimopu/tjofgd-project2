module GenerateRadiusTests

open CellLocation
open Xunit

[<Fact>]
let ``generateRadius zero`` () =
    let radius = 0<cell>

    let expected = 
        [(0<cell>,0<cell>)]
        |> Seq.map (fun e-> e ||> CellLocation.make)
        |> Set.ofSeq

    let actual =
        radius
        |> generateRadius

    Assert.False(expected <> actual)

[<Fact>]
let ``generateRadius one`` () =
    let radius = 1<cell>

    let expected = 
        [(0<cell>, 0<cell>);
         (1<cell>, 0<cell>);
         (-1<cell>, 0<cell>);
         (0<cell>, 1<cell>);
         (0<cell>, -1<cell>)]
        |> Seq.map (fun e-> e ||> CellLocation.make)
        |> Set.ofSeq

    let actual =
        radius
        |> generateRadius

    Assert.False(expected <> actual)

[<Fact>]
let ``generateRadius two`` () =
    let radius = 2<cell>

    let expected = 
        [(0<cell>, 0<cell>);
         (1<cell>, 0<cell>);
         (2<cell>, 0<cell>);
         (-1<cell>, 0<cell>);
         (-2<cell>, 0<cell>);
         (0<cell>, 1<cell>);
         (0<cell>, 2<cell>);
         (0<cell>, -1<cell>);
         (0<cell>, -2<cell>);
         (-1<cell>, 1<cell>);
         (-1<cell>, -1<cell>);
         (1<cell>, 1<cell>);
         (1<cell>, -1<cell>)]
        |> Seq.map (fun e-> e ||> CellLocation.make)
        |> Set.ofSeq

    let actual =
        radius
        |> generateRadius

    Assert.False(expected <> actual)
        

[<Fact>]
let ``generateRadius three`` () =
    let radius = 3<cell>

    let expected = 
        [(0<cell>, 0<cell>);
         (1<cell>, 0<cell>);
         (2<cell>, 0<cell>);
         (3<cell>, 0<cell>);
         (-1<cell>, 0<cell>);
         (-2<cell>, 0<cell>);
         (-3<cell>, 0<cell>);
         (0<cell>, 1<cell>);
         (0<cell>, 2<cell>);
         (0<cell>, 3<cell>);
         (0<cell>, -1<cell>);
         (0<cell>, -2<cell>);
         (0<cell>, -3<cell>);
         (-1<cell>, 1<cell>);
         (-1<cell>, -1<cell>);
         (1<cell>, 1<cell>);
         (1<cell>, -1<cell>);
         (-2<cell>, 2<cell>);
         (-2<cell>, -2<cell>);
         (2<cell>, 2<cell>);
         (2<cell>, -2<cell>);
         (1<cell>, 2<cell>);
         (1<cell>, -2<cell>);
         (-1<cell>, 2<cell>);
         (-1<cell>, -2<cell>);
         (2<cell>, 1<cell>);
         (2<cell>, -1<cell>);
         (-2<cell>, 1<cell>);
         (-2<cell>, -1<cell>)]
        |> Seq.map (fun e-> e ||> CellLocation.make)
        |> Set.ofSeq

    let actual =
        radius
        |> generateRadius

    Assert.False(expected <> actual)


[<Fact>]
let ``generateRadius four`` () =
    let radius = 4<cell>

    let expected = 
        [(0<cell>, 0<cell>);
         (1<cell>, 0<cell>);
         (2<cell>, 0<cell>);
         (3<cell>, 0<cell>);
         (4<cell>, 0<cell>);
         (-1<cell>, 0<cell>);
         (-2<cell>, 0<cell>);
         (-3<cell>, 0<cell>);
         (-4<cell>, 0<cell>);
         (0<cell>, 1<cell>);
         (0<cell>, 2<cell>);
         (0<cell>, 3<cell>);
         (0<cell>, 4<cell>);
         (0<cell>, -1<cell>);
         (0<cell>, -2<cell>);
         (0<cell>, -3<cell>);
         (0<cell>, -4<cell>);
         (-1<cell>, 1<cell>);
         (-1<cell>, -1<cell>);
         (1<cell>, 1<cell>);
         (1<cell>, -1<cell>);
         (-2<cell>, 2<cell>);
         (-2<cell>, -2<cell>);
         (2<cell>, 2<cell>);
         (2<cell>, -2<cell>);
         (1<cell>, 2<cell>);
         (1<cell>, -2<cell>);
         (-1<cell>, 2<cell>);
         (-1<cell>, -2<cell>);
         (2<cell>, 1<cell>);
         (2<cell>, -1<cell>);
         (-2<cell>, 1<cell>);
         (-2<cell>, -1<cell>);
         (3<cell>, 1<cell>);
         (3<cell>, 2<cell>);
         (1<cell>, 3<cell>);
         (2<cell>, 3<cell>);
         (3<cell>, -1<cell>);
         (3<cell>, -2<cell>);
         (1<cell>, -3<cell>);
         (2<cell>, -3<cell>);
         (-3<cell>, 1<cell>);
         (-3<cell>, 2<cell>);
         (-1<cell>, 3<cell>);
         (-2<cell>, 3<cell>);
         (-3<cell>, -1<cell>);
         (-3<cell>, -2<cell>);
         (-1<cell>, -3<cell>);
         (-2<cell>, -3<cell>)]
        |> Seq.map (fun e-> e ||> CellLocation.make)
        |> Set.ofSeq

    let actual =
        radius
        |> generateRadius

    Assert.False(expected <> actual)
