using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteUIManager : MonoBehaviour
{
    //public GameEvent OnShowNote;
    Canvas note;
    public GameEvent OnBeginRecord;

    private void Start()
    {
        note = GetComponent<Canvas>();
        note.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            MenuOnPaused();
            //OnShowNote.Raise();
        }

        if (note.enabled)
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                OnBeginRecord.Raise();
            }
        }
    }

    public void MenuOnPaused()
    {
        note.enabled = !note.enabled;
    }
}
