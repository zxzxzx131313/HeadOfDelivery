using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderHandler : MonoBehaviour
{
    Collider2D collider;
    [SerializeField] private bool PreCondition = false;
    [SerializeField]
    private UnityEvent OnEnterAction;
    [SerializeField] private bool OneHit = true;
    private void Start()
    {
        collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Headtile") || collision.CompareTag("Head"))
        {
            if (PreCondition)
            {
                OnEnterAction.Invoke();
                if (OneHit)
                    collider.enabled = false;
            }
                
        }
    }

    public void CompletePrecondition()
    {
        PreCondition = true;
    }
}
