module GameMenuEncounterDetails

open GameState

let createGameMenuEncounterDetail (playState:PlayState<_>) :EncounterDetail =
    let choices = 
        [{Text="Resume";    Response=Common Cancel};
        {Text="Quit";       Response=GameCommand GameCommandType.Quit};
        {Text="Save...";    Response=GameCommand Save};
        {Text="Load...";    Response=GameCommand Load};
        {Text="New";        Response=GameCommand New};
        {Text="Options..."; Response=Common Cancel}]

    let location = getBoatLocation playState

    {Location=location;
    Title="Game Menu";
    Type=EncounterType.Menu Game;
    Message=["Make your selection:"];
    Choices=choices;
    CurrentChoice=0} 


