open Random
open Game

let private randomFunc (random:System.Random) (parameter:RandomParameter) : RandomResult =
    match parameter with
    | NextInt            -> random.Next()        |> Int
    | MaxInt max         -> random.Next(max)     |> Int
    | IntRange (min,max) -> random.Next(min,max) |> Int
    | NextFloat          -> random.NextDouble()  |> Float

let private random = new System.Random()

[<EntryPoint>]
let main argv = 
    random
    |> randomFunc
    |> runGame

    0

