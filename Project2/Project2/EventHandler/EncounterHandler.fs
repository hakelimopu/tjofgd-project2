module EncounterHandler

open CellLocation
open GameState
open MapObject
open RenderCell

//TODO: start here!

let internal generateStormPCEncounter (location:CellLocation) :Encounters option =
    {Location=location;
    Title="Storm!";
    Type=RanIntoStorm;
    Message=["You have run into a storm;";"it has damaged your boat!"];
    Choices=[{Text="OK";Response=Confirm}];
    CurrentChoice=0} 
    |> PCEncounter 
    |> Some

let internal generateIslandPCEncounter (location:CellLocation) :Encounters option =
    {Location=location;
    Title="Island!";
    Type=DockedWithIsland;
    Message=["You docked at an island!";"What would you like to do?"];
    Choices=
        [{Text="Cast Off!";Response=Cancel};
        {Text="Repair Ship";Response=Repair}];
    CurrentChoice=0} 
    |> PCEncounter 
    |> Some

let internal startPCEncounter (location:CellLocation) (state:PlayState) :Encounters option =
    let actor = state.Actors.[location]
    match actor.Detail with
    | Storm stormProperties -> location |>  generateStormPCEncounter
    | Island islandProperties -> location |> generateIslandPCEncounter
    | _ -> None

let private renderEncounterDetail (details:EncounterDetail)  (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let preparedGrid = 
        renderGrid
        |> clearRectangle {Column=0<cell>;Row=0<cell>} {Column=30<cell>;Row=2<cell> + (getEncounterDetailRows details)} RenderCellColor.Black RenderCellColor.Black 0x20uy
    let g,p = 
        ((preparedGrid |> writeText {Column=1<cell>;Row=1<cell>} RenderCellColor.Blue RenderCellColor.Black details.Title,{Column=1<cell>;Row=3<cell>}), details.Message)
        ||> List.fold (fun (grid,position) line -> (grid |> writeText position RenderCellColor.White RenderCellColor.Black line ,{position with Row=position.Row+1<cell>}))
    let finalGrid, _, _ =
        ((g,{p with Row=p.Row+1<cell>},0),details.Choices)
        ||> List.fold(fun (grid,position,counter) choice -> 
            ((if counter=details.CurrentChoice then (grid |> writeText position RenderCellColor.Cyan RenderCellColor.BrightYellow choice.Text) else (grid |> writeText position RenderCellColor.Cyan RenderCellColor.DarkGray choice.Text)),{position with Row=position.Row+1<cell>},counter+1))
    finalGrid

let internal renderEncounter (state:PlayState) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    match state.Encounters with
    | Some (PCEncounter details) -> renderEncounterDetail details renderGrid
    | Some (NPCEncounters (head::tail)) -> renderEncounterDetail head renderGrid
    | _ -> renderGrid


