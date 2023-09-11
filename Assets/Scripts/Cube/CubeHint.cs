using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CubeHint : MonoBehaviour
{
    [Header("Dice Face Image Components")]
    [SerializeField]
    Image UpImage;
    [SerializeField]
    Image DownImage;
    [SerializeField]
    Image LeftImage;
    [SerializeField]
    Image RightImage;

    [SerializeField]
    Sprite _base;
    [SerializeField]
    Sprite _colored;

    HeadDice _dice;

    // Start is called before the first frame update
    void Start()
    {
        _dice = GetComponentInParent<HeadDice>();
        //_base = Resources.Load<Sprite>("Art/tilemap01for_unity_40");
        //_colored = Resources.Load<Sprite>("Art/rolling01_8");

        RightImage.enabled = false;
        LeftImage.enabled = false;
        UpImage.enabled = false;
        DownImage.enabled = false;
    }

    public void ShowAllHint()
    {
        ShowFace(_dice.GetFaceByPosition(DiceFaceCode.Left), LeftImage);
        ShowFace(_dice.GetFaceByPosition(DiceFaceCode.Right), RightImage);
        ShowFace(_dice.GetFaceByPosition(DiceFaceCode.Bottom), DownImage);
        ShowFace(_dice.GetFaceByPosition(DiceFaceCode.Above), UpImage);

    }

    void ShowFace(Diceface face, Image image)
    {
        if (face.IsColored)
        {

            image.sprite = _colored;
            image.color = new Vector4(1f, 1f, 1f, 0.8f);
        }
        else
        {

            image.sprite = _base;
            image.color = new Vector4(1f, 1f, 1f, 0.5f);
        }

        image.enabled = true;
    }

    public void HideAllHint()
    {
        Image[] imgs = GetComponentsInChildren<Image>();
        foreach (Image img in imgs)
            img.enabled = false;
    }
}
