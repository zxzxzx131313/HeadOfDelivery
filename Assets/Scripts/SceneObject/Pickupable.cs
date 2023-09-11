using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [SerializeField] protected LevelStats stats;
    [SerializeField] protected GameStateSave states;

    public virtual void OnPickup()
    {
        states.AddPickupables(this);
        gameObject.SetActive(false);
        Debug.Log("pickup");
    }

}
