using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueMessager : MonoBehaviour
{
    [SerializeField] StandardUISubtitlePanel panel;
    public void ContinueSubtitle()
    {
        panel.Close();
        Sequencer.Message("ContinueMessage");
    }

    public void SaveResponse()
    {
        panel.Close();
        Sequencer.Message("SaveResponse");
    }
}
