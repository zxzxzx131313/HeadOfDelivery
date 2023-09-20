using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;

public class CutscenePlayer : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector director;
    [SerializeField]
    private UnityEvent OnSceneEnd;
    PlayerController player;
    [SerializeField]
    private Collider2D collider;
    [SerializeField]
    private PlayableDirector playAfter;
    [SerializeField]
    private int playAtLevel;
    public GameStateSave state;
    public LevelStats stats;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Body").GetComponent<PlayerController>();
        //collider = GetComponent<Collider2D>();
        if (collider)
            collider.enabled = true;
    }

    private void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
        if (playAfter != null)
        {
            playAfter.stopped += OnPlayAfterFinished;
        }
    }

    private void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
        if (playAfter != null)
        {
            playAfter.stopped -= OnPlayAfterFinished;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Body"))
        {
            player.OnDisable();
            director.Play();
            state.IsPlaying = true;
        }
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (aDirector == director)
        {
            if (collider)
                collider.enabled = false;
            OnSceneEnd?.Invoke();
            state.IsPlaying = false;
        }
    }

    void OnPlayAfterFinished(PlayableDirector aDirector)
    {
        if (stats.Level == playAtLevel)
        {
            if (aDirector == playAfter)
            {
                player.OnDisable();
                director.Play();
                state.IsPlaying = true;
            }
        }
        
    }
}
