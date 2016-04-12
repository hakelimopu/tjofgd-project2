module EncounterChoiceUtilities

open GameState

let internal clearEncounters (playState:PlayState<_>) :PlayState<_> =
    {playState with Encounters = None}

let private moveToNextChoice (details:EncounterDetail) : EncounterDetail =
    if details.CurrentChoice = details.Choices.Length - 1 then
        details
    else
        let nextChoice = details.CurrentChoice + 1
        let nextWindowIndex = 
            if (nextChoice >= details.WindowIndex + details.WindowSize - 1) && (details.WindowIndex + details.WindowSize < details.Choices.Length) then
                details.WindowIndex + 1
            else
                details.WindowIndex
        {details with CurrentChoice= nextChoice; WindowIndex = nextWindowIndex}

let internal nextEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) -> details |> moveToNextChoice |> PCEncounter |> Some
    | Some (NPCEncounters (head :: tail)) -> (head |> moveToNextChoice) :: tail |> NPCEncounters |> Some
    | _ -> encounter

let private moveToPreviousChoice (details:EncounterDetail) : EncounterDetail =
    if details.CurrentChoice = 0 then
        details
    else
        let previousChoice = details.CurrentChoice - 1
        let nextWindowIndex = 
            if (previousChoice <= details.WindowIndex) && (details.WindowIndex > 0) then
                details.WindowIndex - 1
            else
                details.WindowIndex
        {details with CurrentChoice= previousChoice; WindowIndex = nextWindowIndex}

let internal previousEncounterChoice (encounter: Encounters option) :Encounters option =
    match encounter with
    | Some (PCEncounter details) -> details |> moveToPreviousChoice |> PCEncounter |> Some
    | Some (NPCEncounters (head :: tail)) -> (head |> moveToPreviousChoice) :: tail |> NPCEncounters |> Some
    | _ -> encounter




