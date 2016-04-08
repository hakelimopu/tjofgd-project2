module MainMenuEncounterDetails

open GameState
open CellLocation
open MapObject
open EncounterDetailUtilities

let createMainMenuEncounterDetail (playState:PlayState<_>) :EncounterDetail =
    let choices = 
        [{Text="Resume";    Response=Common Cancel};
        {Text="Boat...";    Response=EncounterReponse.Menu MenuType.Boat};
        {Text="Islands..."; Response=EncounterReponse.Menu MenuType.Island};
        {Text="Game...";    Response=EncounterReponse.Menu MenuType.Game}]

    let location = getBoatLocation playState

    {Location=location;
    Title="Main Menu";
    Type=EncounterType.Menu Main;
    Message=["Make your selection:"];
    Choices=choices;
    CurrentChoice=0} 


