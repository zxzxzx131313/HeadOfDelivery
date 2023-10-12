using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cutsceneCam;

    [SerializeField]
    private CinemachineVirtualCamera followCam;

    [SerializeField]
    private Transform followTarget;

    public GameStateSave states;

    bool play_begin = false;
    bool play_end = false;
    void Start()
    {
        SwitchCutsceneCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (states.IsPlaying && !play_begin)
        {
            play_begin = true;
            play_end = false;
            SwitchCutsceneCamera();
            //followCam.m_Follow = cutsceneCam.transform;

        }
        else if (!states.IsPlaying && !play_end)
        {
            play_begin = false;
            play_end = true;
            SwitchFollowCamera();
            //followCam.m_Follow = followTarget;
        }
        if (!states.IsPlaying)
        {
            cutsceneCam.transform.position = followCam.transform.position;
        }
        else
        {

            followCam.transform.position = cutsceneCam.transform.position;
        }
    }

    void SwitchCutsceneCamera()
    {
        cutsceneCam.enabled = true;
        followCam.enabled = false;
    }

    void SwitchFollowCamera()
    {
        cutsceneCam.enabled = false;
        followCam.enabled = true;
    }
}
