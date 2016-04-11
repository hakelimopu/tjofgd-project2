module OptionsMenuEncounterDetails

open GameState

let createOptionsMenuEncounterDetail (playState:PlayState<_>) :EncounterDetail =
    let choices = 
        [{Text="Resume";    Response=Common Cancel}]

    let location = getBoatLocation playState

    {Location=location;
    Title="Options Menu";
    Type=EncounterType.Menu Options;
    Message=["Make your selection:"];
    Choices=choices;
    CurrentChoice=0} 


