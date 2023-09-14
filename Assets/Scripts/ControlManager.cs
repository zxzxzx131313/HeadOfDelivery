using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class ControlManager : MonoBehaviour
{

    #region EVENTS
    [SerializeField] private GameEvent OnRestartLevel;
    [SerializeField] private GameEvent OnLevelEnd;
    [SerializeField] private GameEvent OnPaused;
    #endregion

    #region GAME STATS
    [SerializeField] private GameStateSave state;
    [SerializeField] private LevelStats stats;
    #endregion  
    #region SCENE OBJECTS
    [SerializeField] private NoteUIManager note;
    [SerializeField] private PlayableDirector GameOpenDirector;
    [SerializeField] private Animator FinalBoard;
    [SerializeField] private LoadScene load;
    #endregion

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            GameOpenDirector.time = GameOpenDirector.duration;
        }
        

        if (!state.IsPlaying)
        {

            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                note.ToggleNote();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame && !state.IsLevelComplete(stats.Level))
            {
                OnRestartLevel.Raise();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                OnPaused.Raise();
            }
        }
    }

    public void CheckEndingResponse()
    {
        StartCoroutine("WaitForResponse");
    }

    IEnumerator WaitForResponse()
    {
        while (!Keyboard.current.anyKey.wasPressedThisFrame)
        {
            yield return null;
        }
        FinalBoard.SetTrigger("End");
        load.BackToTitle();
    }
}
