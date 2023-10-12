using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{

    // testing
    [SerializeField] private int _beginAtLevel = 1;
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave state;
    [SerializeField] private Cutscene Scene;
    [SerializeField] private GameEvent OnNextLevelStart;
    [SerializeField] private GameEvent OnEnterStore;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (stats.Level < _beginAtLevel && _beginAtLevel > 0)
        //{
        //    stats.Level = _beginAtLevel;
        //    //Scene.PlayAnimation();
        //    Invoke("RaiseNextLevelEvent", 1f);
        //}
    }

    void RaiseNextLevelEvent()
    {

        OnNextLevelStart.Raise();
    }

    public void SetCurrentLevelAnimationPlayed()
    {
        state.SetLevelAnimationPlayed(stats.Level);
    }

    public void CheckEndingSummaryResponse()
    {
        Invoke("DelayCallWait", 0.5f);
    }

    void DelayCallWait()
    {
        StartCoroutine("WaitForResponse");
    }

    IEnumerator WaitForResponse()
    {
        
        while (!Keyboard.current.anyKey.wasPressedThisFrame)
        {
            yield return null;
        }
        //FinalBoard.SetTrigger("End");
        //OnEnterStore.Raise();
        //load.BackToTitle();
    }
}
