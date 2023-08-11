using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepCanvas : MonoBehaviour
{
    bool _visible = false;
    Canvas canvas;


    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (transform.parent.GetComponent<PanelManager>().visible != _visible)
        {
            canvas.enabled = false;
            canvas.enabled = true;
            _visible = transform.parent.GetComponent<PanelManager>().visible;
            canvas.enabled = _visible;
        }
    }
}
