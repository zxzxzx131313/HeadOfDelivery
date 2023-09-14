using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum ExpenseType { Token, Elevator };

[CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameSave", order = 0)]
public class GameStateSave : ScriptableObject
{

    [Header("Game States")]
    private bool[] _level_complete_state;
    private bool[] _level_animation_state;
    private List<Interactable> _items;
    private int _money;
    private int _tiles;

    public int StampCount
    {
        get { return _tiles; }
        set
        {
            if (value >= 0)
                _tiles = value;
        }
    }
    private Dictionary<ExpenseType, int> _transactions;
    // Determined if any timeline is playing
    private bool _playing;

    [SerializeField]
    private UnityAction<int> OnMoneyChanged;
    [SerializeField]
    private GameEvent OnEndingLevelComplete;

    public UnityAction<int> OnLevelComplete;

    public void InitState(int levels)
    {
        _level_complete_state = new bool[levels];
        Array.Fill<bool>(_level_complete_state, false);

        _level_animation_state = new bool[levels];
        Array.Fill<bool>(_level_animation_state, false);

        _items = new();
        _money = 0;
        _transactions = new();
    }

    public bool IsLevelComplete(int level)
    {
        return _level_complete_state[level];
    }

    public void SetLevelComplete(int level)
    {
        if (level == _level_complete_state.Length - 2)
            OnEndingLevelComplete.Raise();
        else
            OnLevelComplete?.Invoke(level);
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

    public void AddPickupables(Interactable item)
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

    public bool IsPlaying
    {
        get { return _playing; }
        set
        {
            _playing = value;
        }
    }

    public int TransactionsCount(ExpenseType type)
    {
        if (_transactions.ContainsKey(type))
            return _transactions[type];
        return 0;
    }

    public void AddTransaction(ExpenseType type, int value)
    {
        if (_transactions.ContainsKey(type))
            _transactions[type] += value;
        else
            _transactions[type] = value;
        _money += value;
    }
}
