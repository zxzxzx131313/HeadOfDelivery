using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "LevelStats", menuName = "ScriptableObjects/Stats", order = 0)]
public class LevelStats : ScriptableObject
{
    [Header("Level Setting")]
    [SerializeField] private int _total_level = 5;
    [SerializeField] private int[] _level_steps;
    // potential different level width setting
    [SerializeField] private int _levelPanningOffset = 2;
    [SerializeField] private int _hookDropOffDistance = 15;
    [SerializeField] private float _animationBeginTimeOffset = 0.7f;

    private int _current_level;
    private int _current_steps;

    // use these actions for UI events, other events are handled by GameEvent Objects
    public UnityAction<int> LevelChanged;
    public UnityAction<int> StepsLeftChanged;

    public int Level { get
        {
            return _current_level;
        }
        set
        {
            if (value <= _total_level)
            {
                _current_level = value;
                _current_steps = _level_steps[_current_level];

                LevelChanged?.Invoke(_current_level);
            }
        }
    }

    public int StepsLeft { get
        {
            return _current_steps;
        } set { 
            if (value >= 0)
            {
                _current_steps = value;
                StepsLeftChanged?.Invoke(_current_steps);
            }
        } 
    }

    public int LevelPanningOffset { get { return _levelPanningOffset; } }
    public int HookDropOffDistance { get { return _hookDropOffDistance; } }
    public float AnimationBeginTimeOffset { get { return _animationBeginTimeOffset; } }


    public void InitStats()
    {
        _current_level = 0;
        _current_steps = _level_steps[0];
    }

    public void RestartLevel()
    {
        StepsLeft = _level_steps[_current_level];
    }

}
