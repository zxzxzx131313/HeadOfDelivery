using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HelpManager : MonoBehaviour, IDeselectHandler
{
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject NextButton;
    [SerializeField] private TMP_Text pageCount;
    [SerializeField] private GameObject[] pages;

    public GameEvent OnShowHint;

    int curr_page = 0;
    void Start()
    {
        pageCount.text = (curr_page + 1) + "/" + pages.Length;
        previousButton.SetActive(false);
        //pages[0].enabled = true;
        GetComponent<Canvas>().enabled = false;
    }


    public void SetSelect()
    {
        ShowHelp();
        OnShowHint.Raise();
        LeanTween.move(GetComponent<RectTransform>(), new Vector2(-220f, 83f), 0.5f).setEaseOutCirc();
    }


    public void OnDeselect(BaseEventData data)
    {

        LeanTween.move(GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f).setEaseOutCirc();
        Debug.Log("deselect" + data.selectedObject.name);

        GetComponent<Canvas>().enabled = false;
    }


    public void NextPage()
    {
        curr_page++;
        pageCount.text = (curr_page + 1) + "/" + pages.Length;

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        pages[curr_page].SetActive(true);

        if (curr_page == pages.Length - 1)
        {
            NextButton.SetActive(false);
        }
        else
        {
            NextButton.SetActive(true);
        }
        previousButton.SetActive(true);
    }

    public void PreviousPage()
    {
        curr_page--;
        pageCount.text = (curr_page + 1) + "/" + pages.Length;

        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        pages[curr_page].SetActive(true);

        if (curr_page == 0)
        {
            previousButton.SetActive(false);
        }
        else
        {
            previousButton.SetActive(true);
        }
        NextButton.SetActive(true);
    }

    public void ShowHelp()
    {
        GetComponent<Canvas>().enabled = true;
        LeanTween.move(GetComponent<RectTransform>(), new Vector2(-220f+65f, 83f), 0.5f).setEaseOutCirc();
        pageCount.text = (curr_page + 1) + "/" + pages.Length;
        previousButton.SetActive(false);
        NextButton.SetActive(true);
        pages[0].SetActive(true);
    }

    public void HideHelp()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }

        LeanTween.move(GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f).setEaseOutCirc();
        GetComponent<Canvas>().enabled = false;
    }
}
