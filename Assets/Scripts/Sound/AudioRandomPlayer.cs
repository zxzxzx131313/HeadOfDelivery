using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioRandomPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource[] sources;
    [SerializeField] private AudioSource single_play;

    public void PlayRandom()
    {
        int index = Random.Range(0, sources.Length - 1);
        sources[index].Play();
    }

    public void PlaySingle()
    {
        single_play.Play();
    }
}
