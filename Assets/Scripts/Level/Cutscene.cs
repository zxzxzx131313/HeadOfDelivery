using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene : MonoBehaviour
{
    public PlayableDirector _director;
    public GameEvent OnLevelAnimationEnd;
    public GameEvent OnShowHint;
    public GameEvent OnHideHint;
    public DropPoints points;
    public LevelStats stats;
    public GameStateSave state;

    GameObject _head;

    private void Start()
    {
        
        _head = GameObject.FindGameObjectWithTag("Head");
    }
    public void PlayAnimation()
    {
        TimelineAsset timeline = _director.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            try
            {
                AnimationTrack animation_track = (AnimationTrack)track;
                animation_track.trackOffset = TrackOffset.ApplySceneOffsets;

                Debug.Log(animation_track.name);
            }
            catch (Exception e) { Debug.LogWarning(e); }
        }
        _director.Play();
        state.IsPlaying = true;
    }

    private void OnEnable()
    {
        _director.stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        _director.stopped -= OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {

        if (_director == aDirector)
        {

            OnLevelAnimationEnd.Raise();
            state.IsPlaying = false;
        }
    }

    // called by signal object in cutscene timeline
    public void MoveToDropPoint()
    {
        Vector2 point = points.GetDropPointInLevel(stats.Level);

        _head.transform.position = new Vector3(point.x, _head.transform.position.y, _head.transform.position.z);

        LeanTween.move(_head, point, 0.7f).setEaseInCubic();

        Invoke("ShowInitialHint", 0.7f);
    }

    void ShowInitialHint()
    {

        OnShowHint.Raise();
        //_head.GetComponent<CubeController>().ResetHintTimer();
        // keep showing hint until made the first move
    }

}
