using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BucketManager : MonoBehaviour
{

    [SerializeField]
    private PlayableDirector director;

    PlayerController player;
    public GameStateSave state;
    public LevelStats stats;
    private void Start()
    {

        player = GameObject.FindGameObjectWithTag("Body").GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (aDirector == director)
        {
            state.IsPlaying = false;
        }
    }

    public void PlayAnimation()
    {
        if (stats.ExtraStepsLeft > 0 && stats.Level > 1)
        {
            player.OnDisable();
            director.Play();
            state.IsPlaying = true;
        }
    }
}
