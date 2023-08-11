using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Rendering;

public class PreviewCameraSetting : MonoBehaviour
{
    [SerializeField] private Camera cam;
    Vector2 s;
    public void SetPreviewZoom(Vector2Int size, int index, Vector3 panelScale)
    {
        cam.transform.localPosition = new Vector3((float)1 / 16 * index, (float)-1 / 16 * index, 0);
        s = size;
        float scale = Mathf.Min((float)cam.pixelWidth / size.x, (float)cam.pixelHeight / size.y);
        //Debug.Log(cam.pixelWidth);
        //Debug.Log(cam.pixelHeight);
        Debug.Log("panel" + panelScale + " scale" + Mathf.Min(scale * 1 / 3, 1f) + "size " + size);
        s = new Vector2(size.x, size.y) * scale / 3;
        //Debug.Log(new Vector2(size.x, size.y) * scale/3);
        GetComponent<RectTransform>().localScale = panelScale * Mathf.Min( scale*1/3, 1f);
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.DrawCube(transform.position, new Vector3(s.x /16, s.y / 16, 0));
    }
}
