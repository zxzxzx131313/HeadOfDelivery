using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave state;
    void Awake()
    {
        stats.InitStats();
        state.InitState(stats.TotalLevel);
    }

}
