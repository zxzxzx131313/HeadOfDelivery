using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NoteData", menuName = "ScriptableObjects/NoteData", order = 0)]
public class NoteData : ScriptableObject
{
    [Header("Prefab Setting")]
    [SerializeField] private GameObject _upArrow;
    [SerializeField] private GameObject _downArrow;
    [SerializeField] private GameObject _leftArrow;
    [SerializeField] private GameObject _rightArrow;

    [SerializeField] private GameObject _stampFace;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject _button;
    [SerializeField] private GameObject _stepContainer;
    [SerializeField] private string _dirname;

    [Header("Game State")]
    private int _save_id;
    [SerializeField] private int _save_slots;
    public UnityAction SaveChanged;

    public GameObject UpArrow { get => _upArrow; }
    public GameObject DownArrow { get => _downArrow; }
    public GameObject LeftArrow { get => _leftArrow; }
    public GameObject RightArrow { get => _rightArrow; }
    public GameObject StampFace { get => _stampFace; }
    public GameObject Panel { get => _panel; }
    public GameObject Button { get => _button; }
    public GameObject StepContainer { get => _stepContainer; }
    public string SaveDataDirname { get => _dirname; }

    public int SaveID { 
        get { return _save_id; }
        set { 
            if (value < _save_slots)
            {
                _save_id = value;

                SaveChanged?.Invoke();
            }
        }
    }

}