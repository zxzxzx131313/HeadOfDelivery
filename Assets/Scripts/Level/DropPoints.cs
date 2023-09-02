using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropPoints : MonoBehaviour
{
    public GameObject[] DropPointsInLevel;

    public Vector2 GetDropPointInLevel(int level)
    {
        return (Vector2)DropPointsInLevel[level].transform.position;
    }
}
