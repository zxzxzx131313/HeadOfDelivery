using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TokenUtiles : MonoBehaviour
{
    [SerializeField] private TMP_Text display;
    [SerializeField] private GameStateSave states;
    public void TokenDisplayer()
    {
        display.text = "2 Token to ride\n";
        display.text += "You have " + states.Money;
        if (states.Money < 2)
        {
            display.text += "\nDraw from pay? ( E )";
        }
        else
        {
            display.text += "\nUse token? ( E )";
        }
    }

}