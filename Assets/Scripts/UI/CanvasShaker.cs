using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasShaker : MonoBehaviour
{
    public void Shake()
    {
        Vector2 pos = GetComponent<RectTransform>().anchoredPosition;
        LeanTween.move(GetComponent<RectTransform>(), new Vector2(pos.x - 2f, pos.y), 0.2f).setEaseShake();
    }
}
