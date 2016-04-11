module MainMenuEncounterDetails

open GameState
open CellLocation
open MapObject
open EncounterDetailUtilities

let createMainMenuEncounterDetail (playState:PlayState<_>) :EncounterDetail =
    let choices = 
        [{Text="Resume";    Response=Common Cancel};
        {Text="Boat...";    Response=EncounterResponse.Menu MenuType.Boat};
        {Text="Islands..."; Response=EncounterResponse.Menu MenuType.Island};
        {Text="Game...";    Response=EncounterResponse.Menu MenuType.Game}]

    let location = getBoatLocation playState

    {Location=location;
    Title="Main Menu";
    Type=EncounterType.Menu Main;
    Message=["Make your selection:"];
    Choices=choices;
    CurrentChoice=0} 


