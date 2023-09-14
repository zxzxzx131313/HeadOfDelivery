using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenBag : Interactable
{
    [SerializeField]
    private int value;

    public override void OnInteract()
    {
        base.OnInteract();
        states.AddTransaction(ExpenseType.Token, value);
    }
}
