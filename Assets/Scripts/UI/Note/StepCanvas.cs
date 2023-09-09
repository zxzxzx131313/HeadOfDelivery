using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCanvas : MonoBehaviour
{
    bool _visible = false;
    Canvas canvas;
    bool attached;

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        attached = false;
    }


    // Update is called once per frame
    void Update()
    {
        try
        {
            if (transform.parent.GetComponent<PanelManager>().visible != _visible)
            {
                canvas.enabled = false;
                canvas.enabled = true;
                _visible = transform.parent.GetComponent<PanelManager>().visible;
                canvas.enabled = _visible;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("SteoCanvas Not attached to panel yet.");
        }
        
    }

    public void SetAttached(bool state)
    {
        attached = state;
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
