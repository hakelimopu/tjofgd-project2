module EncounterHandler

open CellLocation
open GameState
open MapObject
open RenderCell

let internal generateStormPCEncounter (location:CellLocation) :Encounters option =
    {Location=location;
    Title="Storm!";
    Type=RanIntoStorm;
    Message=["You have run into a storm,";"and it has damaged your boat!"];
    Choices=[{Text="OK";Response=Confirm}];
    CurrentChoice=0} 
    |> PCEncounter 
    |> Some

let internal startPCEncounter (location:CellLocation) (state:PlayState) :Encounters option =
    let actor = state.Actors.[location]
    match actor.Detail with
    | Storm stormProperties -> location |>  generateStormPCEncounter
    | _ -> None

let private renderPCEncounter (details:EncounterDetail)  (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let g,p = 
        ((renderGrid |> drawText {Column=0<cell>;Row=0<cell>} RenderCellColor.Blue RenderCellColor.Black details.Title,{Column=0<cell>;Row=1<cell>}), details.Message)
        ||> List.fold (fun (grid,position) line -> (grid |> drawText position RenderCellColor.White RenderCellColor.Black line ,{position with Row=position.Row+1<cell>}))
    let finalGrid, _, _ =
        ((g,p,0),details.Choices)
        ||> List.fold(fun (grid,position,counter) choice -> 
            ((if counter=details.CurrentChoice then (grid |> drawText position RenderCellColor.Cyan RenderCellColor.BrightYellow choice.Text) else (grid |> drawText position RenderCellColor.Cyan RenderCellColor.DarkGray choice.Text)),{position with Row=position.Row+1<cell>},counter+1))
    finalGrid

let internal renderEncounter (state:PlayState) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    match state.Encounters with
    | Some (PCEncounter details) -> renderPCEncounter details renderGrid
    | _ -> renderGrid


