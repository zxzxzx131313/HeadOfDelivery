using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class NoteUIManager : MonoBehaviour
{
    //public GameEvent OnShowNote;
    //Canvas note;
    bool active;
    public GameEvent OnBeginRecord;
    public GameEvent OnRestartLevel;
    public UnityAction OnShowNote;
    [SerializeField] private NoteData data;
    [SerializeField] private LevelStats stats;
    [SerializeField] private NoteAnimation content;

    private void Start()
    {
        //note = GetComponent<Canvas>();
        //note.enabled = false;
        active = false;


    }

    private void OnEnable()
    {
        stats.LevelChanged += NotePanToNextLevel;
    }

    private void OnDisable()
    {
        stats.LevelChanged -= NotePanToNextLevel;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleNote();
            content.ToggleContent();
            OnShowNote?.Invoke();
        }

        if (active)
        {
            if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                OnBeginRecord.Raise();
            }
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                OnRestartLevel.Raise();
            }
        }
    }

    public void ToggleNote()
    {
        //note.enabled = !note.enabled;
        active = !active;

        if (active)
        {
            LeanTween.move(transform.GetChild(0).GetComponent<RectTransform>(), new Vector2(0, 0), 0.5f).setEaseOutCirc();
        }
        else
        {
            LeanTween.move(transform.GetChild(0).GetComponent<RectTransform>(), new Vector2(65, 0), 0.5f).setEaseOutBounce();
        }
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
