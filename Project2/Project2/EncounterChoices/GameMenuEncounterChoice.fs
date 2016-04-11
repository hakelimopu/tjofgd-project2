module GameMenuEncounterChoice

open GameState
open EncounterChoiceUtilities
open System.Runtime.Serialization.Formatters.Binary
open System.IO
open OptionsMenuEncounterDetails

let saveGame (playState: PlayState<_>) : PlayState<_> =
    let playState':PlayState<unit> =
        {Actors = playState.Actors; Encounters = None; MapGrid = playState.MapGrid; RenderData = ()}

    let formatter = new BinaryFormatter()
    use stream = new FileStream("SaveFile.bin", FileMode.Create, FileAccess.Write, FileShare.None)
    formatter.Serialize(stream, playState')

    playState |> clearEncounters

let loadGame  (playState: PlayState<_>):PlayState<_> =
    let formatter = new BinaryFormatter();
    use stream = new FileStream("SaveFile.bin", FileMode.Open, FileAccess.Read, FileShare.Read)
    let loadedPlayState = 
        formatter.Deserialize(stream) :?> PlayState<unit>
    {playState with Actors = loadedPlayState.Actors; Encounters = loadedPlayState.Encounters; MapGrid = loadedPlayState.MapGrid}

let openOptionsMenu  (playState: PlayState<_>) :PlayState<_> =
    {playState with Encounters = createOptionsMenuEncounterDetail playState |> PCEncounter |> Some}

let applyGameMenuPCEncounterChoice (createFunc:unit->GameState<_>) (detail:EncounterDetail) (playState: PlayState<_>) : GameState<_> option =
    match detail |> getEncounterResponse with
    | GameCommand Save                 -> playState |> saveGame |> PlayState |> Some
    | GameCommand Load                 -> loadGame playState |> PlayState |> Some
    | GameCommand New                  ->  createFunc() |> Some
    | EncounterResponse.Menu Options   -> playState |> openOptionsMenu |> PlayState |> Some
    | GameCommand GameCommandType.Quit -> None
    | _                                -> playState|> clearEncounters |> PlayState |> Some


