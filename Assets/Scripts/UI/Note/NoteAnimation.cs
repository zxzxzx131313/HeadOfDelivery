using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RawImage NotePreview;
    public Canvas Sticker;
    public GameObject Board;

    private void Start()
    {

        enabled = false;
    }


    public void ToggleContent(bool IsOpen)
    {
        //if (gameObject.activeInHierarchy) gameObject.SetActive(false);
        //else gameObject.SetActive(true);
        enabled = IsOpen;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Vector2 pos = GetComponent<RectTransform>().anchoredPosition;
        //pos += new Vector2(-60, 70);
        LeanTween.move(Board.GetComponent<RectTransform>(), new Vector2(-160, 90), 0.5f).setEaseOutBounce();
        NotePreview.GetComponent<Canvas>().enabled = false;
        Sticker.enabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Vector2 pos = GetComponent<RectTransform>().anchoredPosition;
        //pos -= new Vector2(-60, 60);


        LeanTween.move(Board.GetComponent<RectTransform>(), Vector2.zero, 0.5f).setEaseOutBounce();
        NotePreview.GetComponent<Canvas>().enabled = true;
        Sticker.enabled = true;
    }

}
