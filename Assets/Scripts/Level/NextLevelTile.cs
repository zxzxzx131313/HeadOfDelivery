using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
public class NextLevelTile : MonoBehaviour
{
    public GameEvent OnNextLevelStart;
    public GameEvent OnChangeLevel;
    PlayerController body;
    bool activated;

    // Note: Tiles are made 2 units high to prevent player jump and enter into the collision area which results in initial starting position offset.
    private void Start()
    {
        activated = false;
        body = GameObject.FindGameObjectWithTag("Body").GetComponent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("next");
        if (collision.CompareTag("Body"))
        {
            if (!activated)
            {
                OnChangeLevel.Raise();
                Invoke("RaiseNextLevelEvent", 0.5f);
                activated = true;
            }
        }
            
    }

    void RaiseNextLevelEvent()
    {
        OnNextLevelStart.Raise();
    }
}
