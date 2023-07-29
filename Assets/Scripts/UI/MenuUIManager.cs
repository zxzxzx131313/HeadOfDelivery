using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuUIManager : MonoBehaviour
{
    public GameEvent OnPaused;
    public Canvas steps;
    Canvas Menu;

    private void Start()
    {
        Menu = GetComponent<Canvas>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OnPaused.Raise();
        }
    }

    public void MenuOnPaused()
    {
        Menu.enabled = !Menu.enabled;
        steps.enabled = !steps.enabled;
    }
}
