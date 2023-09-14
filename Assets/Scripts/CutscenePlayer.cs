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
    Collider2D collider;
    public GameStateSave state;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Body").GetComponent<PlayerController>();
        collider = GetComponent<Collider2D>();
        collider.enabled = true;
    }

    private void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
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
            player.OnEnable();
            collider.enabled = false;
            OnSceneEnd?.Invoke();
            state.IsPlaying = false;
        }
    }
}
