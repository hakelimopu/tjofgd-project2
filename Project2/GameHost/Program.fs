open Random

let randomFunc (random:System.Random) (parameter:RandomParameter) : RandomResult =
    match parameter with
    | NextInt -> random.Next() |> Int
    | MaxInt max -> random.Next(max) |> Int
    | IntRange (min,max) -> random.Next(min,max) |> Int
    | NextFloat -> random.NextDouble() |> Float
    

[<EntryPoint>]
let main argv = 
    let random = new System.Random()

    Game.runGame (randomFunc random)

    0

