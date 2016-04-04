﻿module QueryQuestEncounterDetails

open GameState
open CellLocation
open MapObject

let createQuestQueryEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =

    let choices = 
        [{Text="No problem!"; Response=Confirm};
         {Text="No thanks!";  Response=Cancel}]

    let _, island =  getIsland location playState

    let quest = island.Quest

    let _, island' = getIsland quest.Destination playState

    {Location=location;
    Title="Need a job?";
    Type=EncounterType.QueryQuest;
    Message=["Could you deliver this"; island'.Name |> sprintf "parcel to %s?"; (quest.Reward / 1.0<currency>) |> sprintf "The pay is $%.2f"];
    Choices=choices;
    CurrentChoice=0} 

