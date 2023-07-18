using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogistic : MonoBehaviour
{
    public int LevelCount;

    private void Start()
    {
        LevelCount = 5;
    }

    public int GetLevel()
    {
        return LevelCount;
    }
}
