using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StickerUI : MonoBehaviour
{
    NoteUIManager note;
    Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();    
        canvas.enabled = false;
        note = GameObject.FindGameObjectWithTag("Note").GetComponentInParent<NoteUIManager>();
    }

    private void OnEnable()
    {
        note.OnShowNote += ToggleSticker;
    }

    private void OnDisable()
    {
        note.OnShowNote -= ToggleSticker;
    }

    void ToggleSticker()
    {
        canvas.enabled = !canvas.enabled;
    }
}
