using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class FinalCalculation : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private GameStateSave states;
    [SerializeField] private LevelStats stats;
    [SerializeField]private Tilemap _headTile;

    void Start()
    {
        //foreach (var tm in GameObject.FindObjectsOfType<Tilemap>())
        //{
        //    if (tm.CompareTag("Headtile"))
        //        _headTile = tm;
        //}
    }

    public void SetSummaryData()
    {
        int tile_count = states.StampCount;

        text.text = tile_count + " * 1\n\n";

        int paint_left = stats.ExtraStepsLeft;
        text.text += paint_left + " * 2 \n\n";

        int token = 0;
        if (states.TransactionsCount(ExpenseType.Token) != 0)
        {
            token = states.TransactionsCount(ExpenseType.Token);
        }
        text.text += token + "\n\n";
        int elevator = 0;
        if (states.TransactionsCount(ExpenseType.Elevator) != 0)
        {
            elevator = states.TransactionsCount(ExpenseType.Elevator);
        }
        text.text += elevator + "\n\n";

        int total = tile_count * 1 + paint_left * 2 + token + elevator;

        text.text += total;
    }
}
