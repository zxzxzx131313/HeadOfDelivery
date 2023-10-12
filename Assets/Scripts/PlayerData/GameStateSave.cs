using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;

public enum ExpenseType { Token, Elevator };

[CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameSave", order = 0)]
public class GameStateSave : ScriptableObject
{

    [Header("Game States")]
    private bool[] _level_complete_state;
    private bool[] _level_animation_state;
    private List<Interactable> _items;
    private int _tiles;
    private int _money;
    private List<FaceAbilityCode> _abilities;

    public float MasterVolume;
    public float EffectVolume;
    public float BGMVolume;

    void Start()
    {
        MasterVolume = 0.8f;
        EffectVolume = 0.8f;
        BGMVolume = 0.8f;
    }


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
    private Dictionary<ExpenseType, int> _transactions_temp;
    // Determined if any timeline is playing
    public bool _playing;

    [SerializeField]
    private UnityAction<int> OnMoneyChanged;
    [SerializeField]
    private GameEvent OnEndingLevelComplete;

    public UnityAction<int> OnLevelComplete;

    public void InitGame()
    {
        _money = 0;
    }

    public void InitState(int levels)
    {
        _level_complete_state = new bool[levels];
        Array.Fill<bool>(_level_complete_state, false);

        _level_animation_state = new bool[levels];
        Array.Fill<bool>(_level_animation_state, false);

        _items = new();
        _tiles = 0;
        _transactions = new();
        _transactions_temp = new();
        if (_abilities.Count == 0)
        { 
            _abilities = new();
            _abilities.Add(FaceAbilityCode.Base);
        }
    }

    public bool HasAbility(FaceAbilityCode ability)
    {
        return _abilities.Contains(ability);
    }

    public void AddNewAbility(FaceAbilityCode ability)
    {
        if (!_abilities.Contains(ability))
            _abilities.Add(ability);
        else
            Debug.LogWarning("Try to add existing ability.");
    }

    public FaceAbilityCode[] GetAllAbilities()
    {
        FaceAbilityCode[] ability = new FaceAbilityCode[_abilities.Count];
        _abilities.CopyTo(ability);
        return ability;
    }

    public bool IsLevelComplete(int level)
    {
        return _level_complete_state[level];
    }

    public void SetLevelComplete(int level)
    {
        if (level == _level_complete_state.Length - 3)
            OnEndingLevelComplete.Raise();
        else
            OnLevelComplete?.Invoke(level);
        _level_complete_state[level] = true;
        _items = new();
        foreach (var transaction in _transactions_temp)
        {
            if (_transactions.ContainsKey(transaction.Key))
                _transactions[transaction.Key] += transaction.Value;
            else
                _transactions[transaction.Key] = transaction.Value;
        }
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
        get {
            return _transactions.Values.ToList().Sum();
        }
        set
        {
            _money = Money;
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
        if (_transactions_temp.ContainsKey(type))
            _transactions_temp[type] += value;
        else
            _transactions_temp[type] = value;
        //_money += value;
    }

    public void RespawnItems()
    {
        foreach (var item in _items)
        {
            item.gameObject.SetActive(true);
            _transactions_temp = new();
        }
    }
}
