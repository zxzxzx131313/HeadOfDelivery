using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Interactable
{
    [SerializeField]
    private int value;

    public override void OnInteract()
    {

        //states.AddPickupables(this);
        states.AddTransaction(ExpenseType.Elevator, value);
    }
}
