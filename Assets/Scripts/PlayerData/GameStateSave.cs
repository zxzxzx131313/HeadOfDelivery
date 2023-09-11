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
    private List<Pickupable> _items;
    private int _money;

    [SerializeField]
    private UnityAction<int> OnMoneyChanged;
    [SerializeField]
    private GameEvent OnEndingLevelComplete;

    public void InitState(int levels)
    {
        _level_complete_state = new bool[levels];
        Array.Fill<bool>(_level_complete_state, false);

        _level_animation_state = new bool[levels];
        Array.Fill<bool>(_level_animation_state, false);

        _items = new();
        _money = 0;
    }

    public bool IsLevelComplete(int level)
    {
        return _level_complete_state[level];
    }

    public void SetLevelComplete(int level)
    {
        if (level == _level_complete_state.Length - 1)
            OnEndingLevelComplete.Raise();
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

    public void AddPickupables(Pickupable item)
    {
        _items.Add(item);
    }

    public int Money { 
        get { return _money; }
        set { 
            if (value != _money)
            {
                _money = value;
                OnMoneyChanged?.Invoke(_money);
            }
        }
    }
}
