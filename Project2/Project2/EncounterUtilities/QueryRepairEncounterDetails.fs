module QueryRepairEncounterDetails

open GameState
open CellLocation
open EncounterDetailUtilities
open MapObject

let createQueryRepairEncounterDetail (playState:PlayState<_>) (location:CellLocation) :EncounterDetail =
    let boatProperties = getBoatProperties playState

    let damage = boatProperties.MaximumHull - boatProperties.Hull
    let currency = boatProperties.Wallet

    let choices = 
        [({Text="Yes, please!"; Response=Confirm}, ``always include choice``);
         ({Text="No, thanks!";  Response=Cancel},  ``always include choice``)]
        |> List.filter (filterChoice playState)
        |> List.map fst

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

    {Location=location;
    Title="Repair Ship";
    Type=EncounterType.QueryRepair;
    Message=[sprintf "Repair costs $%.2f @ hull" (island.RepairCost * 1.0<health/currency>);
        sprintf "You can repair %d" (maximumDamageRepaired / 1.0<health> |> int);
        sprintf "Total cost: $%.2f" (totalCost / 1.0<currency>)];
    Choices=choices;
    CurrentChoice=0} 


