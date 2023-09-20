using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private AudioClip open;
    [SerializeField] private AudioClip close;
    [SerializeField] private AudioSource EffectPlayer;

    Animator anim;
    bool AnimationTrigger = false;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void OnOpen()
    {
        EffectPlayer.clip = open;
        EffectPlayer.Play();
    }

    public void OnClose()
    {
        StartCoroutine(WaitAnimation());
    }

    IEnumerator WaitAnimation()
    {
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("door-close") || !AnimationTrigger)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("door-close"))
                AnimationTrigger = true;
            yield return null;
        }
        EffectPlayer.clip = close;
        EffectPlayer.Play();
        AnimationTrigger = false;
    }
}
