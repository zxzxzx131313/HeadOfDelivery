using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private LevelStats stats;
    void Awake()
    {
        stats.InitStats();
    }

}
