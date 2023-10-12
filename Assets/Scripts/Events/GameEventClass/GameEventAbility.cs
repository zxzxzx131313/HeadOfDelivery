using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEventAbility : ScriptableObject
{
	private List<GameEventListenerAbility> listeners =
		new List<GameEventListenerAbility>();

	public void Raise(FaceAbilityCode value)
	{
		for (int i = listeners.Count - 1; i >= 0; i--)
        {
			listeners[i].OnEventRaised(value);
		}
	}

	public void RegisterListener(GameEventListenerAbility listener)
	{ listeners.Add(listener); }

	public void UnregisterListener(GameEventListenerAbility listener)
	{ listeners.Remove(listener); }
}
