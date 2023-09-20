using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenBag : Interactable
{
    [SerializeField]
    private int value;
    [SerializeField] private AudioClip sound;
    [SerializeField] private AudioSource EffectPlayer;

    public override void OnInteract()
    {
        base.OnInteract();
        states.AddTransaction(ExpenseType.Token, value);

        EffectPlayer.clip = sound;
        EffectPlayer.Play();
    }
}
