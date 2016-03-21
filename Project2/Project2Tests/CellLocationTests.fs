[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage>]
module CellLocationTests

open Xunit
open CellLocation

let private assertSumLocationsTest (worldSize:CellLocation) (first:CellLocation) (second:CellLocation) (expected:CellLocation) =
    let actual =
        CellLocation.sumLocationsWrapped worldSize first second
    Assert.Equal (expected,actual)

let private throwsSumLocationsTest<'T when 'T :> exn> (worldSize:CellLocation) (first:CellLocation) (second:CellLocation) =
    let doTest () =     
        CellLocation.sumLocationsWrapped worldSize first second
        |> ignore

    Assert.Throws<'T>(doTest)

[<Fact>]
let ``from location add positive value to x and y but do not wrap`` () =
    assertSumLocationsTest {Column=100<cell>;Row=110<cell>} {Column=10<cell>;Row=20<cell>} {Column=30<cell>;Row=40<cell>} {Column=40<cell>;Row=60<cell>} 

[<Fact>]
let ``from a location add positive x and negative y and wrap to the north`` () =
    assertSumLocationsTest {Column=100<cell>;Row=110<cell>} {Column=10<cell>;Row=20<cell>} {Column=30<cell>;Row= -40<cell>} {Column=40<cell>;Row=90<cell>} 

[<Fact>]
let ``from a location add positive x and y and wrap to the south`` () =
    assertSumLocationsTest {Column=100<cell>;Row=110<cell>} {Column=10<cell>;Row=40<cell>} {Column=30<cell>;Row= 75<cell>} {Column=40<cell>;Row=5<cell>} 

[<Fact>]
let ``from a location add negative x and y and wrap to the west`` () =
    assertSumLocationsTest {Column=100<cell>;Row=110<cell>} {Column=10<cell>;Row=20<cell>} {Column= -30<cell>;Row=40<cell>} {Column=80<cell>;Row=60<cell>} 

[<Fact>]
let ``from a location add positive x and y and wrap to the east`` () =
    assertSumLocationsTest {Column=100<cell>;Row=110<cell>} {Column=90<cell>;Row=20<cell>} {Column=30<cell>;Row=40<cell>} {Column=20<cell>;Row=60<cell>} 

[<Fact>]
let ``sum locations zero world width`` () =
    throwsSumLocationsTest<CellLocation.InvalidWorldSize> {Column=0<cell>;Row=110<cell>} {Column=10<cell>;Row=20<cell>} {Column=30<cell>;Row=40<cell>}

[<Fact>]
let ``sum locations zero world height`` () =
    throwsSumLocationsTest<CellLocation.InvalidWorldSize> {Column=100<cell>;Row=0<cell>} {Column=10<cell>;Row=20<cell>} {Column=30<cell>;Row=40<cell>}

[<Fact>]
let ``sum locations negative world width`` () =
    throwsSumLocationsTest<CellLocation.InvalidWorldSize> {Column= -100<cell>;Row= 110<cell>} {Column=10<cell>;Row=20<cell>} {Column=30<cell>;Row=40<cell>}

[<Fact>]
let ``sum locations negative world height`` () =
    throwsSumLocationsTest<CellLocation.InvalidWorldSize> {Column= 100<cell>;Row= -110<cell>} {Column=10<cell>;Row=20<cell>} {Column=30<cell>;Row=40<cell>}

let private assertWrapLocationTest (worldSize:CellLocation) (location:CellLocation) (expected:CellLocation) =
    let actual = 
        wrapLocation worldSize location

    Assert.Equal (expected,actual)

let private throwsWrapLocationTest<'T when 'T :> exn> (worldSize:CellLocation) (location:CellLocation) (expected:string) =
    let doTest () =     
        wrapLocation worldSize location
        |> ignore

    Assert.Throws<'T>(doTest)

[<Fact>]
let ``wrap location no wrapping`` () =
    assertWrapLocationTest {Column=100<cell>;Row=110<cell>} {Column=50<cell>; Row=55<cell>} {Column=50<cell>; Row=55<cell>}

[<Fact>]
let ``wrap location zero width`` () =
    throwsWrapLocationTest<CellLocation.InvalidWorldSize> {Column=0<cell>;Row=110<cell>} {Column=50<cell>; Row=55<cell>}

[<Fact>]
let ``wrap location zero height`` () =
    throwsWrapLocationTest<CellLocation.InvalidWorldSize> {Column=100<cell>;Row=0<cell>} {Column=50<cell>; Row=55<cell>}

[<Fact>]
let ``wrap location negative width`` () =
    throwsWrapLocationTest<CellLocation.InvalidWorldSize> {Column= -100<cell>;Row=110<cell>} {Column=50<cell>; Row=55<cell>}

[<Fact>]
let ``wrap location negative height`` () =
    throwsWrapLocationTest<CellLocation.InvalidWorldSize> {Column=100<cell>;Row= -110<cell>} {Column=50<cell>; Row=55<cell>}

[<Fact>]
let ``wrap location positive x`` () =
    assertWrapLocationTest {Column=100<cell>;Row=110<cell>} {Column=150<cell>; Row=55<cell>} {Column=50<cell>; Row=55<cell>}

[<Fact>]
let ``wrap location negative x`` () =
    assertWrapLocationTest {Column=100<cell>;Row=110<cell>} {Column= -50<cell>; Row=55<cell>} {Column=50<cell>; Row=55<cell>}

[<Fact>]
let ``wrap location positive y`` () =
    assertWrapLocationTest {Column=100<cell>;Row=110<cell>} {Column=50<cell>; Row=155<cell>} {Column=50<cell>; Row=45<cell>}

[<Fact>]
let ``wrap location negative y`` () =
    assertWrapLocationTest {Column=100<cell>;Row=110<cell>} {Column=50<cell>; Row= -55<cell>} {Column=50<cell>; Row=55<cell>}

let private assertDistanceFormulaTestTest (worldSize:CellLocation) (maximum:int<cell>) (first:CellLocation) (second:CellLocation) (expected:bool) =
    let actual =
        CellLocation.distanceFormulaTestWrapped worldSize maximum first second
    Assert.Equal (expected,actual)

let private throwsDistanceFormulaTestTest<'T when 'T :> exn> (worldSize:CellLocation) (maximum:int<cell>) (first:CellLocation) (second:CellLocation) =
    let doTest () =     
        CellLocation.distanceFormulaTestWrapped worldSize maximum first second
        |> ignore

    Assert.Throws<'T>(doTest)

[<Fact>]
let ``DistanceFormulaTestWrapped - zero world width`` () =
    throwsDistanceFormulaTestTest<CellLocation.InvalidWorldSize> {Column=0<cell>;Row=110<cell>} 0<cell> {Column=0<cell>;Row=0<cell>} {Column=0<cell>;Row=0<cell>}

[<Fact>]
let ``DistanceFormulaTestWrapped - zero world height`` () =
    throwsDistanceFormulaTestTest<CellLocation.InvalidWorldSize> {Column=100<cell>;Row=0<cell>} 0<cell> {Column=0<cell>;Row=0<cell>} {Column=0<cell>;Row=0<cell>}

[<Fact>]
let ``DistanceFormulaTestWrapped - negative world width`` () =
    throwsDistanceFormulaTestTest<CellLocation.InvalidWorldSize> {Column= -100<cell>;Row=110<cell>} 0<cell> {Column=0<cell>;Row=0<cell>} {Column=0<cell>;Row=0<cell>}

[<Fact>]
let ``DistanceFormulaTestWrapped - negative world height`` () =
    throwsDistanceFormulaTestTest<CellLocation.InvalidWorldSize> {Column= 100<cell>;Row= -110<cell>} 0<cell> {Column=0<cell>;Row=0<cell>} {Column=0<cell>;Row=0<cell>}

[<Fact>]
let ``DistanceFormulaTestWrapped - negative maximum`` () =
    throwsDistanceFormulaTestTest<CellLocation.NegativeValue> {Column= 100<cell>;Row= 110<cell>} -1<cell> {Column=0<cell>;Row=0<cell>} {Column=0<cell>;Row=0<cell>}

[<Fact>]
let ``DistanceFormulaTestWrapped - zero maximum and zero distance`` () =
    assertDistanceFormulaTestTest {Column= 100<cell>;Row= 110<cell>} 0<cell> {Column=0<cell>;Row=0<cell>} {Column=0<cell>;Row=0<cell>} false


[<Fact>]
let ``DistanceFormulaTestWrapped - zero maximum and one distance`` () =
    assertDistanceFormulaTestTest {Column= 100<cell>;Row= 110<cell>} 0<cell> {Column=0<cell>;Row=0<cell>} {Column=1<cell>;Row=0<cell>} true
