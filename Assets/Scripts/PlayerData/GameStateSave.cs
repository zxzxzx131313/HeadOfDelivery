using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameSave", order = 0)]
public class GameStateSave : ScriptableObject
{

    [Header("Game States")]
    private bool[] _level_complete_state;
    private bool[] _level_animation_state;

    public void InitState(int levels)
    {
        _level_complete_state = new bool[levels];
        Array.Fill<bool>(_level_complete_state, false);

        _level_animation_state = new bool[levels];
        Array.Fill<bool>(_level_animation_state, false);
    }

    public bool IsLevelComplete(int level)
    {
        return _level_complete_state[level];
    }

    public void SetLevelComplete(int level)
    {
        _level_complete_state[level] = true;
    }

    public bool IsLevelAnimationPlayed(int level)
    {
        return _level_animation_state[level];
    }

    public void SetLevelAnimationPlayed(int level)
    {
        _level_animation_state[level] = true;
    }
}
