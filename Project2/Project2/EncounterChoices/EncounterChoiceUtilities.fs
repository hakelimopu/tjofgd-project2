module EncounterChoiceUtilities

open GameState

let internal clearEncounters (playState:PlayState<_>) :PlayState<_> =
    {playState with Encounters = None}

let internal nextEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) ->{details with CurrentChoice= (details.CurrentChoice + 1) % (details.Choices.Length)} |> PCEncounter |> Some
    | Some (NPCEncounters (head :: tail)) -> {head with CurrentChoice= (head.CurrentChoice + 1) % (head.Choices.Length)} :: tail |> NPCEncounters |> Some
    | _ -> encounter

let internal previousEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) ->{details with CurrentChoice= (details.CurrentChoice + details.Choices.Length - 1) % (details.Choices.Length)} |> PCEncounter |> Some
    | Some (NPCEncounters (head :: tail)) -> {head with CurrentChoice= (head.CurrentChoice + head.Choices.Length - 1) % (head.Choices.Length)} :: tail |> NPCEncounters |> Some
    | _ -> encounter




