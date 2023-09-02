using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private PlayableDirector first;
    [SerializeField] private PlayableDirector second;

    public void PlayLoopAnimation()
    {
        TimelineAsset timeline = second.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            try
            {
                AnimationTrack animation_track = (AnimationTrack)track;
                //if (animation_track.name == "End Track (2)")
                //animation_track.trackOffset = TrackOffset.ApplySceneOffsets;


            }
            catch (Exception e) { Debug.LogWarning(e); }
        }
        second.Play();
    }

    private void OnEnable()
    {
        first.stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        first.stopped -= OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {

        if (first == aDirector)
        {

            PlayLoopAnimation();
        }
    }
}
