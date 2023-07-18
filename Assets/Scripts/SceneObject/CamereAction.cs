using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamereAction : MonoBehaviour
{
    public void PanToNextLevel()
    {
        var cam = Camera.main;
        float halfHeight = cam.orthographicSize;
        float width = halfHeight * cam.aspect * 2;
        Vector3 pos = transform.position;
        pos.x += width - 2f;
        //transform.position = pos;
        LeanTween.moveX(gameObject, pos.x, 0.5f).setEase(LeanTweenType.easeInQuad);
    }
}