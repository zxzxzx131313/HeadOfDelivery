using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteToggleButton : MonoBehaviour
{
    NoteUIManager note;
    //bool IsOpen;
    [SerializeField] private Sprite CloseStateSprite;
    [SerializeField] private Sprite OpenStateSprite;
    void Start()
    {
        GetComponent<Image>().sprite = CloseStateSprite;
        //IsOpen = false;

    }

    private void Awake()
    {
        note = GameObject.FindGameObjectWithTag("Note").GetComponentInParent<NoteUIManager>();
        
    }

    private void OnEnable()
    {
        note.OnShowNote += ToggleSprite;
    }


    private void OnDisable()
    {
        note.OnShowNote -= ToggleSprite;

    }

    void ToggleSprite(bool IsOpen)
    {
        if (IsOpen)
            Deactivate();
        else
            Activate();
        //IsOpen = !IsOpen;
    }

    void Activate()
    {
        GetComponent<Image>().sprite = CloseStateSprite;
        //IsOpen = true;
    }

    void Deactivate()
    {

        GetComponent<Image>().sprite = OpenStateSprite;
        //IsOpen = false;
    }
}
