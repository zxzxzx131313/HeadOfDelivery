using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PanelButton : MonoBehaviour
{
    public int Index;
    public UnityAction<int> Clicked;
    [SerializeField] private Sprite ActiveSprite;
    [SerializeField] private Sprite DeactiveSprite;

    public void SendIndexOnClick()
    {
        Clicked?.Invoke(Index);
        ActivateSprite();
    }

    public void ActivateSprite()
    {
        GetComponent<Image>().sprite = ActiveSprite;
    }

    public void DeactivateSprite()
    {
        GetComponent<Image>().sprite = DeactiveSprite;
    }
}
