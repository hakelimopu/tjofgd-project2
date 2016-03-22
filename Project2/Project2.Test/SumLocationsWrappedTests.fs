[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute>]
module SumLocationsWrappedTests

open Xunit
open CellLocation

[<Fact>]
let ``sumLocationsWrapped both locations with positive x and y and no wrapping`` () =
    let worldSize = {Column = 100<cell>; Row = 110<cell>}
    let first     = {Column =  10<cell>; Row =  20<cell>}
    let second    = {Column =  30<cell>; Row =  40<cell>}
    let expected  = {Column =  40<cell>; Row =  60<cell>}

    let actual = sumLocationsWrapped worldSize first second

    Assert.Equal(expected, actual)

[<Fact>]
let ``sumLocationsWrapped wrap on negative x axis`` () =
    let worldSize = {Column = 100<cell>; Row = 110<cell>}
    let first     = {Column =  10<cell>; Row =  20<cell>}
    let second    = {Column = -30<cell>; Row =  40<cell>}
    let expected  = {Column =  80<cell>; Row =  60<cell>}

    let actual = sumLocationsWrapped worldSize first second

    Assert.Equal(expected, actual)

[<Fact>]
let ``sumLocationsWrapped wrap on positive x axis`` () =
    let worldSize = {Column = 100<cell>; Row = 110<cell>}
    let first     = {Column =  10<cell>; Row =  20<cell>}
    let second    = {Column =  90<cell>; Row =  40<cell>}
    let expected  = {Column =   0<cell>; Row =  60<cell>}

    let actual = sumLocationsWrapped worldSize first second

    Assert.Equal(expected, actual)

[<Fact>]
let ``sumLocationsWrapped wrap on negative y axis`` () =
    let worldSize = {Column = 100<cell>; Row = 110<cell>}
    let first     = {Column =  10<cell>; Row =  20<cell>}
    let second    = {Column =  30<cell>; Row = -40<cell>}
    let expected  = {Column =  40<cell>; Row =  90<cell>}

    let actual = sumLocationsWrapped worldSize first second

    Assert.Equal(expected, actual)

[<Fact>]
let ``sumLocationsWrapped wrap on positive y axis`` () =
    let worldSize = {Column = 100<cell>; Row = 110<cell>}
    let first     = {Column =  10<cell>; Row =  20<cell>}
    let second    = {Column =  30<cell>; Row =  90<cell>}
    let expected  = {Column =  40<cell>; Row =   0<cell>}

    let actual = sumLocationsWrapped worldSize first second

    Assert.Equal(expected, actual)

[<Fact>]
let ``sumLocationsWrapped with zero world width`` () =
    let worldSize = {Column =   0<cell>; Row = 110<cell>}
    let first     = {Column =  10<cell>; Row =  20<cell>}
    let second    = {Column =  30<cell>; Row =  40<cell>}
    let expected  = {Column =  40<cell>; Row =  60<cell>}

    Assert.Throws<InvalidWorldSize>(fun ()->sumLocationsWrapped worldSize first second |> ignore)

[<Fact>]
let ``sumLocationsWrapped with zero world height`` () =
    let worldSize = {Column = 100<cell>; Row =   0<cell>}
    let first     = {Column =  10<cell>; Row =  20<cell>}
    let second    = {Column =  30<cell>; Row =  40<cell>}
    let expected  = {Column =  40<cell>; Row =  60<cell>}

    Assert.Throws<InvalidWorldSize>(fun ()->sumLocationsWrapped worldSize first second |> ignore)


[<Fact>]
let ``sumLocationsWrapped with negative world width`` () =
    let worldSize = {Column = -100<cell>; Row = 110<cell>}
    let first     = {Column =   10<cell>; Row =  20<cell>}
    let second    = {Column =   30<cell>; Row =  40<cell>}
    let expected  = {Column =   40<cell>; Row =  60<cell>}

    Assert.Throws<InvalidWorldSize>(fun ()->sumLocationsWrapped worldSize first second |> ignore)

[<Fact>]
let ``sumLocationsWrapped with negative world height`` () =
    let worldSize = {Column = 100<cell>; Row = -110<cell>}
    let first     = {Column =  10<cell>; Row =   20<cell>}
    let second    = {Column =  30<cell>; Row =   40<cell>}
    let expected  = {Column =  40<cell>; Row =   60<cell>}

    Assert.Throws<InvalidWorldSize>(fun ()->sumLocationsWrapped worldSize first second |> ignore)
