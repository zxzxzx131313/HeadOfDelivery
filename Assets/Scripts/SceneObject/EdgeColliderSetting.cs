using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeColliderSetting : MonoBehaviour
{
    EdgeCollider2D edge;

    public LevelStats stats;

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

    private void OnEnable()
    {
        stats.LevelChanged += CollisionAndCameraPanToNextLevel;
    }

    private void OnDisable()
    {
        stats.LevelChanged -= CollisionAndCameraPanToNextLevel;
    }

    void CollisionAndCameraPanToNextLevel(int level)
    {
        var cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float width = halfHeight * cam.aspect * 2 - stats.LevelPanningOffset;
        for (int i = 0; i < edge.points.Length; i++)
            edge.points[i].x += width;
        Vector3 pos = transform.position;
        pos.x += width;
        transform.position = pos;
        // camera transition
        Vector3 cam_pos = cam.gameObject.transform.position;
        cam_pos.x += width;
        LeanTween.moveX(cam.gameObject, cam_pos.x, 0.5f).setEase(LeanTweenType.easeInQuad);
    }
}
