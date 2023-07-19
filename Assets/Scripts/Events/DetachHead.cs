using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachHead : MonoBehaviour
{
    public GameEvent OnDetachHead;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("head");
        if (collision.CompareTag("Body"))
            OnDetachHead.Raise();
    }
}
