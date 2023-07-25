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
    public DropPoints points;
    public LevelStats stats;

    GameObject _head;

    public void PlayAnimation()
    {
        _director.Play();
        _head = GameObject.FindGameObjectWithTag("Head");
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
            TimelineAsset timeline = _director.playableAsset as TimelineAsset;
            foreach (var track in timeline.GetOutputTracks())
            {
                try
                {
                    AnimationTrack animation_track = (AnimationTrack)track;
                    animation_track.trackOffset = TrackOffset.ApplySceneOffsets;
                }
                catch (Exception e) { Debug.LogWarning(e); }   
            }  
        }
    }

    public void MoveToDropPoint()
    {
        Vector2 point = points.GetDropPointInLevel(stats.Level);

        _head.transform.position = new Vector2(point.x, _head.transform.position.y);

        LeanTween.move(_head, point, 0.7f).setEaseInCubic();
    }
}
