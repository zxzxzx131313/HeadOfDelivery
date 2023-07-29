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
    public string _dirpath;

    public GameObject UpArrow { get => _upArrow; }
    public GameObject DownArrow { get => _downArrow; }
    public GameObject LeftArrow { get => _leftArrow; }
    public GameObject RightArrow { get => _rightArrow; }
    public GameObject StampFace { get => _stampFace; }
    public string SaveDataDirname { get => _dirpath; }

}