[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute>]
module DistanceFormulaTestWrappedTests

open Xunit
open System
open CellLocation

[<Fact>]
let ``distanceFormulaTestWrapped zero world width`` () =
    let worldSize = {Column = 0<cell>;Row = 110<cell>}
    let maximum = 0<cell>
    let first = {Column = 0<cell>;Row = 0<cell>}
    let second = {Column = 0<cell>;Row = 0<cell>}

    Assert.Throws<InvalidWorldSize>(fun ()->distanceFormulaTestWrapped worldSize maximum first second |> ignore)

[<Fact>]
let ``distanceFormulaTestWrapped zero world height`` () =
    let worldSize = {Column = 100<cell>;Row = 0<cell>}
    let maximum = 0<cell>
    let first = {Column = 0<cell>;Row = 0<cell>}
    let second = {Column = 0<cell>;Row = 0<cell>}

    Assert.Throws<InvalidWorldSize>(fun ()->distanceFormulaTestWrapped worldSize maximum first second |> ignore)

[<Fact>]
let ``distanceFormulaTestWrapped negative world width`` () =
    let worldSize = {Column = -100<cell>;Row = 110<cell>}
    let maximum = 0<cell>
    let first = {Column = 0<cell>;Row = 0<cell>}
    let second = {Column = 0<cell>;Row = 0<cell>}

    Assert.Throws<InvalidWorldSize>(fun ()->distanceFormulaTestWrapped worldSize maximum first second |> ignore)

[<Fact>]
let ``distanceFormulaTestWrapped negative world height`` () =
    let worldSize = {Column = 100<cell>;Row = -110<cell>}
    let maximum = 0<cell>
    let first = {Column = 0<cell>;Row = 0<cell>}
    let second = {Column = 0<cell>;Row = 0<cell>}

    Assert.Throws<InvalidWorldSize>(fun ()->distanceFormulaTestWrapped worldSize maximum first second |> ignore)

[<Fact>]
let ``distanceFormulaTestWrapped negative maximum`` () =
    let worldSize = {Column = 100<cell>;Row = 110<cell>}
    let maximum = -1<cell>
    let first = {Column = 0<cell>;Row = 0<cell>}
    let second = {Column = 0<cell>;Row = 0<cell>}

    Assert.Throws<NegativeValue>(fun ()->distanceFormulaTestWrapped worldSize maximum first second |> ignore)

[<Fact>]
let ``distanceFormulaTestWrapped zero maximum zero distance`` () =
    let worldSize = {Column = 100<cell>;Row = 110<cell>}
    let maximum = 0<cell>
    let first = {Column = 0<cell>;Row = 0<cell>}
    let second = {Column = 0<cell>;Row = 0<cell>}

    let expected = false
    
    let actual = distanceFormulaTestWrapped worldSize maximum first second

    Assert.Equal(expected, actual)

[<Fact>]
let ``distanceFormulaTestWrapped zero maximum one distance`` () =
    let worldSize = {Column = 100<cell>;Row = 110<cell>}
    let maximum = 0<cell>
    let first = {Column = 0<cell>;Row = 0<cell>}
    let second = {Column = 1<cell>;Row = 0<cell>}

    let expected = true
    
    let actual = distanceFormulaTestWrapped worldSize maximum first second

    Assert.Equal(expected, actual)

