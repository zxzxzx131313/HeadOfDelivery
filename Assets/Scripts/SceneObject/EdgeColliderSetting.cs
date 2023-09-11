using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EdgeColliderSetting : MonoBehaviour
{
    EdgeCollider2D edge;

    public LevelStats stats;
    public GameObject Cam;

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

        var edgePoints = new[] {topLeft, bottomLeft };
        edge.points = edgePoints;


        edge = gameObject.AddComponent<EdgeCollider2D>();
        edgePoints = new[] { topRight, bottomRight };
        edge.points = edgePoints;



    }

    private void OnEnable()
    {
        //stats.LevelChanged += CollisionAndCameraPanToNextLevel;
    }

    private void OnDisable()
    {
        //stats.LevelChanged -= CollisionAndCameraPanToNextLevel;
    }

    public void CollisionAndCameraPanToNextLevel(Vector2 direction)
    {
        var cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float width = halfHeight * cam.aspect * 2 - stats.LevelPanningOffset;
        //for (int i = 0; i < edge.points.Length; i++)
        //    edge.points[i].x += width - stats.LevelPanningOffset;
        //Vector3 pos = transform.position;
        //pos.x += width * direction.x;
        //pos.y += halfHeight * 2 * direction.y;
        //transform.position = pos;
        // camera transition
        //Cam.GetComponent<CinemachineVirtualCamera>().AddCinemachineComponent<cinemachine>
        Vector3 cam_pos = Cam.transform.position;
        cam_pos.x += width * direction.x;
        cam_pos.y += halfHeight * 2 * direction.y;
        LeanTween.move(Cam, cam_pos, 0.5f).setEase(LeanTweenType.easeInQuad);

        transform.position = cam_pos;
    }

    public void MoveCameraUpOneScreen()
    {
        CollisionAndCameraPanToNextLevel(Vector2.up);
    }

    public void MoveCameraDownOneScreen()
    {
        CollisionAndCameraPanToNextLevel(Vector2.down);
    }
}
