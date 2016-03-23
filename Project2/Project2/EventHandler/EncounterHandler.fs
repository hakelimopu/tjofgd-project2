module EncounterHandler

open CellLocation
open GameState
open MapObject
open RenderCell
open EncounterDetails

let internal generateStormPCEncounter (location:CellLocation) :Encounters option =
    location 
    |> createStormEncounterDetail
    |> PCEncounter 
    |> Some

let internal generateIslandPCEncounter (playState:PlayState) (location:CellLocation) :Encounters option =
    location
    |> createIslandEncounterDetail playState
    |> PCEncounter 
    |> Some

//TODO: pass in the properties to the generators!
let internal startPCEncounter (location:CellLocation) (state:PlayState) :Encounters option =
    let actor = 
        state.Actors.[location]

    match actor.Detail with
    | Storm stormProperties   -> location |> generateStormPCEncounter
    | Island islandProperties -> location |> generateIslandPCEncounter state
    | _                       -> None

let private renderEncounterDetail (details:EncounterDetail)  (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    let upperLeft = 
        {Column = 0<cell>;
         Row    = 0<cell>}

    let size = 
        {Column = 30<cell>;
         Row    =  2<cell> + (getEncounterDetailRows details)}

    let titleLocation = {Column=1<cell>;Row=1<cell>}

    let messageLocation = {Column=1<cell>;Row=3<cell>}

    let preparedGrid = 
        renderGrid
        |> clearRectangle upperLeft size RenderCellColor.Black RenderCellColor.Black 0x20uy

    let titledGrid =
        preparedGrid 
        |> writeText titleLocation RenderCellColor.Blue RenderCellColor.Black details.Title

    let writeMessageLine (grid,location) line =
        let updatedGrid = 
            grid 
            |> writeText location RenderCellColor.White RenderCellColor.Black line 

        let updatedLocation = 
            {location with Row=location.Row+1<cell>}

        (updatedGrid, updatedLocation)

    let grid,postMessageLocation = 
        ((titledGrid,messageLocation), details.Message)
        ||> List.fold writeMessageLine

    let choiceLocation = {postMessageLocation with Row=postMessageLocation.Row+1<cell>}

    let writeChoiceLine (grid,position,counter) choice = 
        let updatedGrid = 
            if counter=details.CurrentChoice then 
                grid 
                |> writeText position RenderCellColor.Cyan RenderCellColor.BrightYellow choice.Text
            else 
                grid 
                |> writeText position RenderCellColor.Cyan RenderCellColor.DarkGray     choice.Text

        let updatedPosition = 
            {position with Row=position.Row+1<cell>}

        let updatedCounter = 
            counter+1

        (updatedGrid, updatedPosition, updatedCounter)

    let fst3 (x,y,z) = x

    ((grid,choiceLocation,0),details.Choices)
    ||> List.fold writeChoiceLine
    |> fst3

let internal renderEncounter (state:PlayState) (renderGrid:CellMap<RenderCell>) :CellMap<RenderCell> =
    match state.Encounters with
    | Some (PCEncounter details)        -> renderEncounterDetail details renderGrid
    | Some (NPCEncounters (head::tail)) -> renderEncounterDetail head    renderGrid
    | _                                 -> renderGrid


