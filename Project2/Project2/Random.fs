module Random

open System.Diagnostics.CodeAnalysis

[<ExcludeFromCodeCoverageAttribute>]
type RandomParameter = 
    | NextInt
    | MaxInt of int
    | IntRange of int * int
    | NextFloat

[<ExcludeFromCodeCoverageAttribute>]
type RandomResult =
    | Int of int
    | Float of float

[<ExcludeFromCodeCoverageAttribute>]
exception WrongRandomResultType

[<ExcludeFromCodeCoverageAttribute>]
type RandomFunc = RandomParameter -> RandomResult

let getInt (result:RandomResult) : int =
    match result with
    | Int value -> value
    | _ -> raise WrongRandomResultType

let getFloat (result:RandomResult) : float =
    match result with
    | Float value -> value
    | _ -> raise WrongRandomResultType

let randomIntRange (random:RandomFunc) (min:int, max:int) :int =
    (min, max) |> IntRange |> random |> getInt

    