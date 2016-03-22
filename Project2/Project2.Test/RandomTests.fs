[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute>]
module RandomTests

open Xunit

[<Fact>]
let ``RandomResult Int passed to getInt`` () = 
    let initial = 0 |> Random.Int

    let expected = 0

    let actual =
        initial
        |> Random.getInt

    Assert.Equal(expected, actual)

[<Fact>]
let ``RandomResult Float passed to getInt`` () = 
    let initial = 0.0 |> Random.Float

    Assert.Throws<Random.WrongRandomResultType>(fun ()-> initial |> Random.getInt |> ignore)

[<Fact>]
let ``RandomResult Int passed to getFloat`` () = 
    let initial = 0 |> Random.Int

    Assert.Throws<Random.WrongRandomResultType>(fun ()-> initial |> Random.getFloat |> ignore)

[<Fact>]
let ``RandomResult Float passed to getFloat`` () = 
    let initial = 0.0 |> Random.Float

    let expected = 0.0

    let actual =
        initial
        |> Random.getFloat

    Assert.Equal(expected, actual)
