module QueryRepairEncounterDetails

open GameState
open CellLocation
open EncounterDetailUtilities
open MapObject

let getRepairDetails (location:CellLocation) (playState:PlayState<_>) : float<currency/health> * float<health> * float<currency> =
    let boatProperties = getBoatProperties playState

    let damage = boatProperties.MaximumHull - boatProperties.Hull
    let currency = boatProperties.Wallet

    let _, island =  getIsland location playState

    let costPerHealth = 1.0<health> * island.RepairCost

    let maximumDamageRepaired =
        (damage,currency)
        |> Seq.unfold (fun (damage,currency)->
            if damage <= 0<health> || currency < costPerHealth then
                None
            else
                Some (1.0<health>, (damage - 1<health>, currency - costPerHealth)))
        |> Seq.reduce (+)

    let totalCost = maximumDamageRepaired * island.RepairCost
    (island.RepairCost, maximumDamageRepaired, totalCost)


let createQueryRepairEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =
    let repairCost, maximumDamageRepaired, totalCost = getRepairDetails location playState

    let choices = 
        [{Text="Yes, please!"; Response=Confirm};
         {Text="No, thanks!";  Response=Cancel}]

    {Location=location;
    Title="Repair Ship";
    Type=EncounterType.QueryRepair;
    Message=[sprintf "Repair costs $%.2f @ hull" (repairCost * 1.0<health/currency>);
        sprintf "You can repair %d" (maximumDamageRepaired / 1.0<health> |> int);
        sprintf "Total cost: $%.2f" (totalCost / 1.0<currency>)];
    Choices=choices;
    CurrentChoice=0} 


