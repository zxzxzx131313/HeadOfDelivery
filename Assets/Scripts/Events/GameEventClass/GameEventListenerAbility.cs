using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerAbility : ScriptableObject
{
    public GameEventAbility Event;
    public UnityEvent<FaceAbilityCode> Response;

    private void OnEnable()
    { Event.RegisterListener(this); }

    private void OnDisable()
    { Event.UnregisterListener(this); }

    public void OnEventRaised(FaceAbilityCode value)
    { Response.Invoke(value); }
}