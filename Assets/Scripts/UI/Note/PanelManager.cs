using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PanelManager : MonoBehaviour
{

    public int Index { get; private set;}
    public Canvas canvas;
    public TMP_Text steps;

    public bool visible = false;
    //bool canvasState = false;
    bool noteCanvas = false;
    bool activated = false;
    NoteUIManager note;

    private void Awake()
    {
        //canvas = GetComponent<Canvas>();    
        canvas.sortingLayerName = "UI";
        canvas.enabled = false;
        note = GameObject.FindGameObjectWithTag("Note").GetComponentInParent<NoteUIManager>();
    }

    private void OnEnable()
    {
        note.OnShowNote += TogglePanelCannvas;
    }

    private void OnDisable()
    {
        note.OnShowNote -= TogglePanelCannvas;
    }

    public void SetIndex(int index)
    {
        Index = index;
        GetButton().GetComponent<PanelButton>().Index = index;
        RectTransform rt = GetButton().GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y - index * 16);
        gameObject.GetComponent<RectTransform>().anchoredPosition += new Vector2(1, -1) * index;
    }

    public PanelButton GetButton()
    {
        return GetComponentInChildren<PanelButton>();
    }

    public void SetStepDisplay(int stepsCount)
    {
        steps.text = stepsCount.ToString();
    }

    public StepRecorder GetStepRecorder()
    {
        return GetComponentInChildren<StepRecorder>();
    }

    public void SetActive()
    {
        //ameObject.SetActive(true);
        //canvasState = true;
        GetButton().ActivateSprite();
        canvas.sortingOrder = 3;
        visible = true;
        activated = true;
        canvas.enabled = true;
    }

    public void Deactivate()
    {
        GetButton().DeactivateSprite();
        canvas.sortingOrder = 0;
        visible = false;
        //gameObject.SetActive(false);
        //canvasState = false;
        canvas.enabled = false;
        activated = false;
    }

    public void WaitForActivation()
    {
        //gameObject.SetActive(true);
        //canvasState = true;
        GetButton().DeactivateSprite();
        canvas.sortingOrder = 2;
        activated = true;
        canvas.enabled = true;
    }

    public void SetToBackLayer()
    {
        canvas.sortingOrder = 0;
        visible = false;
    }

    public void SetToFrontLayer()
    {
        canvas.sortingOrder = 4;
        visible = true;
    }

    public bool IsActivated()
    {
        return activated;
    }

    public void TogglePanelCannvas(bool IsOpen)
    {

        noteCanvas = IsOpen;
        UpdateCanvasVisibility();
    }

    public void UpdateCanvasVisibility()
    {
        //trigger repaint
        canvas.enabled = false;
        canvas.enabled = true;
        if (noteCanvas && activated)
            canvas.enabled = true;
        else
            canvas.enabled = false;
    }
}
