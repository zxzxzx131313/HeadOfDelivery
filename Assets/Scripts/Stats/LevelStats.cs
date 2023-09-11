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
    [SerializeField] private DiceFaceCode[] _begin_level_color_face_position;
    // potential different level width setting
    [SerializeField] private int _levelPanningOffset = 2;

    private int _current_level;
    private int _current_steps;
    private int _extra_steps_on_begin;
    private int _extra_steps;

    // use these actions for UI events, other events are handled by GameEvent Objects
    public UnityAction<int> LevelChanged;
    public UnityAction<int> StepsLeftChanged;
    public UnityAction<int> ExtraStepsLeftChanged;


    public int TotalLevel { get { return _total_level; } }

    public int Level { get
        {
            return _current_level;
        }
        set
        {
            if (value <= _total_level && value != _current_level)
            {
                _current_level = value;
                if (value > 0)
                {
                    _extra_steps_on_begin = StepsLeft;
                    _extra_steps = _extra_steps_on_begin;
                }
                _current_steps = _level_steps[_current_level];
                LevelChanged?.Invoke(_current_level);
            }
        }
    }

    public int StepsLeft { get
        {
            return _current_steps + ExtraStepsLeft;
        } set { 
            if (value >= 0)
            {
                if (value >= ExtraStepsLeft)
                {
                    _current_steps = value - ExtraStepsLeft;
                }
                else
                {
                    ExtraStepsLeft = value;
                }
                StepsLeftChanged?.Invoke(_current_steps);
            }
        } 
    }

    public int ExtraStepsLeft
    {
        get
        {
            return _extra_steps;
        }
        set
        {
            if (value >= 0)
            {
                _extra_steps = value;
                ExtraStepsLeftChanged?.Invoke(_current_steps);
            }
        }
    }

    public int ExtraStepLeftOnBegin { get { return _extra_steps_on_begin; } }

    public int[] LevelSteps
    {
        get
        {
            return _level_steps;
        }
    }

    public DiceFaceCode LevelBeginColorFacePosition(int level) { return _begin_level_color_face_position[level]; }

    public int LevelPanningOffset { get { return _levelPanningOffset; } }


    public void InitStats()
    {
        _current_level = 0;
        _current_steps = _level_steps[0];
        _extra_steps = 0;
        _extra_steps_on_begin = 0;
    }

    public void RestartLevel()
    {
        StepsLeft = _level_steps[_current_level] + _extra_steps_on_begin;
    }

}
