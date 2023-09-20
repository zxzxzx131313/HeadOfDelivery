using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] private AudioMixer Mixer;

    private void Start()
    {

        SetMasterVolume(0.8f);
        SetEffectVolume(0.8f);
        SetMBGMVolume(0.8f);
    }

    public void SetMasterVolume(float value)
    {
        float volume;
        if (value == 1f)
            volume = 20f;
        else if (value == 0f)
            volume = -50f;
        else
            volume = (-20f * Mathf.Log(10, value / 10f) -10f);
        Mixer.SetFloat("MasterVolume", volume);
        Debug.Log(volume);
    }

    public void SetMBGMVolume(float value)
    {
        float volume;
        if (value == 1f)
            volume = 20f;
        else if (value == 0f)
            volume = -50f;
        else
            volume = (-20f * Mathf.Log(10, value / 10f) - 10f);
        Mixer.SetFloat("BGMVolume", volume);
        Debug.Log(volume);
    }

    public void SetEffectVolume(float value)
    {
        float volume;
        if (value == 1f)
            volume = 20f;
        else if (value == 0f)
            volume = -50f;
        else
            volume = (-20f * Mathf.Log(10, value / 10f) - 10f);
        Mixer.SetFloat("EffectVolume", volume);
        Debug.Log(volume);
    }
}
