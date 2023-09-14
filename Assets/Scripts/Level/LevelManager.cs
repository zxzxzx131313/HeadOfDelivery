using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    // testing
    [SerializeField] private int _beginAtLevel = 1;
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave state;
    [SerializeField] private Cutscene Scene;
    [SerializeField] private GameEvent OnNextLevelStart;
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
}
