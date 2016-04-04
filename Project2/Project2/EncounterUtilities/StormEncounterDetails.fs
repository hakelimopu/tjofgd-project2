module StormEncounterDetails

open CellLocation
open GameState

let createStormEncounterDetail (location:CellLocation) :EncounterDetail =
    {Location=location;
     Title="Storm!";
     Type=RanIntoStorm;
     Message=["You have run into a storm;";"it has damaged your boat!"];
     Choices=[{Text="OK";Response=Common Confirm}];
     CurrentChoice=0}


