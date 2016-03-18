module Console

open System

let writeLines (lines:String list) :unit =
    let writeLine (line:string) :unit = Console.WriteLine(line)
    lines
    |> List.iter (writeLine)

let rec filterInput (filter:Set<ConsoleKey>) :ConsoleKey=
    let key = Console.ReadKey()
    if filter.Contains(key.Key) then
        key.Key
    else
        filterInput filter


