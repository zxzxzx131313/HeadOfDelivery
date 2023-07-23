using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerBool : MonoBehaviour
{
    public GameEventBool Event;
    public UnityEvent<bool> Response;

    private void OnEnable()
    { Event.RegisterListener(this); }

    private void OnDisable()
    { Event.UnregisterListener(this); }

    public void OnEventRaised(bool value)
    { Response.Invoke(value); }
}