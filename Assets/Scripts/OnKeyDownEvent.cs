using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.InputSystem;


public class OnKeyDownEvent : MonoBehaviour
{

    private void Update()
    {
        if (DialogueManager.IsConversationActive)
        {
            Debug.Log("active");
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                CloseSubtitle();
            }
        }
    }

    public void CloseSubtitle()
    {
        GetComponentInParent<StandardUISubtitlePanel>().Close();
        Sequencer.Message("ClosedSubtitle");
    }
}
