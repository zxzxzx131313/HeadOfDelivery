using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EdgeColliderSetting : MonoBehaviour
{
    EdgeCollider2D edge;

    public LevelStats stats;
    public GameObject Cam;
    CinemachineVirtualCamera vcam;
    CinemachineFramingTransposer vcam_follow;

    void Start()
    {
        //AddCollider();
        var camera = Camera.main;
        var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
        vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        //vcam_follow = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
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

    public Vector3 CollisionAndCameraPanToNextLevel(Vector2 direction)
    {
        var cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float width = halfHeight * cam.aspect * 2 - stats.LevelPanningOffset;

        Vector3 cam_pos = Cam.transform.position;
        cam_pos.x += width * direction.x;
        cam_pos.y += halfHeight * 2 * direction.y;
        LeanTween.move(Cam, cam_pos, 0.5f).setEase(LeanTweenType.easeInQuad);

        transform.position = cam_pos;

        return cam_pos;
        //vcam_follow.m_TrackedObjectOffset = new Vector3(width * direction.x, halfHeight * 2 * direction.y, 0);
    }

    public void CameraToPos(Vector3 pos)
    {
        var cam = Camera.main;
        LeanTween.move(Cam, pos, 0.5f).setEase(LeanTweenType.easeInQuad);

    }

    public void SetFramingOffsetBack()
    {
       //vcam_follow.m_TrackedObjectOffset = Vector3.zero;
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
