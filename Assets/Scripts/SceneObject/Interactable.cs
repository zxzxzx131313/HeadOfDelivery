using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected LevelStats stats;
    [SerializeField] protected GameStateSave states;

    public virtual void OnInteract()
    {
        states.AddPickupables(this);
        gameObject.SetActive(false);
    }

}
