using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : Interactable
{
    [SerializeField]
    private int value;
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource EffectPlayer;


    public override void OnInteract()
    {
        states.AddTransaction(ExpenseType.Elevator, value);
    }

    public void OnOpen()
    {

        EffectPlayer.clip = sound;
        EffectPlayer.Play();
    }


}
