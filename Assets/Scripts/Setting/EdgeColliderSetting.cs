using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeColliderSetting : MonoBehaviour
{
    //int level = 5;
    // Start is called before the first frame update
    EdgeCollider2D edge;
    void Start()
    {
        AddCollider();
    }

    void AddCollider()
    {
        var cam = Camera.main;

        var bottomLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        var topLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
        var topRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        topRight.x += 1;
        var bottomRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));
        bottomRight.x += 1;

        // add or use existing EdgeCollider2D
        edge = GetComponent<EdgeCollider2D>() == null ? gameObject.AddComponent<EdgeCollider2D>() : GetComponent<EdgeCollider2D>();

        var edgePoints = new[] { bottomLeft, topLeft, topRight, bottomRight };
        edge.points = edgePoints;

    }
    public void CollisionPanToNextLevel()
    {
        var cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float width = halfHeight * cam.aspect * 2 - 2f;
        for (int i = 0; i < edge.points.Length; i++)
            edge.points[i].x += width;
        Vector3 pos = transform.position;
        pos.x += width;
        transform.position = pos;
    }
}
