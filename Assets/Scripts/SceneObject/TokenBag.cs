using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenBag : Pickupable
{
    [SerializeField]
    private int value;

    public override void OnPickup()
    {
        base.OnPickup();
        states.Money += value;
    }
}
