using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Cutscene : MonoBehaviour
{
    public PlayableDirector _director;
    public GameEvent OnLevelAnimationEnd;

    public void PlayAnimation()
    {
        _director.Play();
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
            foreach (AnimationTrack track in timeline.GetOutputTracks())
                track.trackOffset = TrackOffset.ApplySceneOffsets;
        }
    }
}
