using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutotialManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private PlayableDirector _complete_director;
    [SerializeField] private GameEvent OnLevelAnimationEnd;
    [SerializeField] private GameEvent OnDetachHead;
    [SerializeField] private GameStateSave state;
    [SerializeField] private LevelStats stats;
    [SerializeField] private HelpManager help;
    [SerializeField] private GameObject restartHint;

    PlayerController body;

    private void Start()
    {
        body = GameObject.FindGameObjectWithTag("Body").GetComponent<PlayerController>();
        restartHint.SetActive(false);
        //_complete_director.playOnAwake = false;
    }

    private void Update()
    {
        //if (state.IsLevelComplete(0))
        //{
        //    _complete_director.Play();
        //    body.GetComponent<PlayerController>().enabled = false;
        //}

        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            _director.time = _director.duration;
        }
    }


    private void OnEnable()
    {
        _director.stopped += StartTutorialLevel;
        stats.StepsLeftChanged += CheckForHideHint;
    }

    private void OnDisable()
    {

        _director.stopped -= StartTutorialLevel;
        stats.StepsLeftChanged -= CheckForHideHint;
    }

    void CheckForHideHint(int step)
    {
        if (stats.StepsLeft == 0 && (stats.Level == 0 || stats.Level == 1))
        {
            restartHint.SetActive(true);
            Invoke("HideHint", 3f);
        }
    }

    void HideHint()
    {
        restartHint.SetActive(false);

    }


    void StartTutorialLevel(PlayableDirector aDirector)
    {
        if (aDirector == _director)
        {
            // set player disable
            body.OnChangeLevel(0);
            // same state as animation end in OnLevelBegin
            OnDetachHead.Raise();
            OnLevelAnimationEnd.Raise();
            help.SetSelect();
        }
    }
}
