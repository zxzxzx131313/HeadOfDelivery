using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowTarget : MonoBehaviour
{
    [SerializeField]
    private Transform body;

    [SerializeField]
    private Transform head;

    public float radius;
    // Update is called once per frame
    void Update()
    {
        Camera cam = Camera.main;
        float dir = body.transform.position.x >= 0 ? 1f : -1f;
        float halfHeight = cam.orthographicSize;
        float width = halfHeight * cam.aspect * 2;
        transform.position = new Vector2((body.transform.position.x + head.transform.position.x) / 2f, body.transform.position.y );
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position, radius);
        Gizmos.color = Color.yellow;
    }
}
