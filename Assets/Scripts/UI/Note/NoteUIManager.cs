using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Playables;

public class NoteUIManager : MonoBehaviour
{
    //public GameEvent OnShowNote;
    //Canvas note;
    bool active;
    public GameEvent OnBeginRecord;
    public GameEvent OnRestartLevel;
    public UnityAction<bool> OnShowNote;
    [SerializeField] private NoteData data;
    [SerializeField] private LevelStats stats;
    [SerializeField] private GameStateSave state;
    [SerializeField] private NoteAnimation content;
    [SerializeField] private Canvas help;

    private void Start()
    {
        //note = GetComponent<Canvas>();
        //note.enabled = false;
        active = false;

        transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(65, 0, 0.5f);
    }

    private void OnEnable()
    {
        //stats.LevelChanged += NotePanToNextLevel;
    }

    private void OnDisable()
    {
        //stats.LevelChanged -= NotePanToNextLevel;
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void ToggleNote()
    {
        //note.enabled = !note.enabled;
        active = !active;

        SetNote(active);
    }

    public void SetNote(bool state)
    {
        active = state;
        if (state)
        {
            LeanTween.move(transform.GetChild(0).GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f).setEaseOutCirc();
        }
        else
        {
            LeanTween.move(transform.GetChild(0).GetComponent<RectTransform>(), new Vector2(65, 0), 0.5f).setEaseOutBounce();
            help.enabled = false;
        }
        content.ToggleContent(active);
        OnShowNote?.Invoke(active);
    }


    void NotePanToNextLevel(int level)
    {
        var cam = Camera.main;

        float halfHeight = cam.orthographicSize;
        float width = halfHeight * cam.aspect * 2 - stats.LevelPanningOffset;

        Vector3 pos = transform.position;
        pos.x = width * level;
        transform.position = pos;
    }
}
