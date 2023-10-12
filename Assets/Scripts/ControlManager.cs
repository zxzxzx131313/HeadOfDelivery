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
    private GameObject Setting;
    #endregion

    void Start()
    {
        Setting = GameObject.FindGameObjectWithTag("Setting");

        //Setting.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {


        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            GameOpenDirector.time = GameOpenDirector.duration-1f;
        }
        

        if (!state.IsPlaying)
        {

            if (Keyboard.current.mKey.wasPressedThisFrame)
            {
                note.ToggleNote();
            }

            if (Keyboard.current.rKey.wasPressedThisFrame && !state.IsLevelComplete(stats.Level) && state.IsLevelAnimationPlayed(stats.Level)) 
            {
                OnRestartLevel.Raise();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Setting.GetComponent<Canvas>().enabled = true;
            }
        }
    }
    
}
