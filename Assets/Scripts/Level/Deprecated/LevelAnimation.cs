//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.Playables;
//using UnityEngine.Timeline;

//public class LevelAnimation : MonoBehaviour
//{
//    public Animator HeadAnimator;
//    public Animator HookAnimator;

//    public TimelineAsset Timeline;
//    public PlayableDirector first;
//    public GameEvent OnLevelAnimationEnd;

//    public LevelStats stats;

//    GameObject _player;

//    //[Header("End Setting")]
//    //[SerializeField]
//    //float AnimationBeginTimeOffset = 0.7f;
//    // Start is called before the first frame update
//    void Start()
//    {
//        _player = GameObject.FindGameObjectWithTag("Body");

//        //first.playableAsset = Timeline;

//        //TimelineAsset asset = first.playableAsset as TimelineAsset;
//        //// note - we're deleting the track if it exists already, since we want to generate everything on the spot for this example
//        //foreach (TrackAsset track in asset.GetOutputTracks())
//        //    asset.DeleteTrack(track);

//        //AnimationTrack new_track = Timeline.CreateTrack<AnimationTrack>("AnimationTrack");
//        //new_track.CreateRecordableClip()
//        //first.SetGenericBinding(new_track, AnimatorObject);
//        //Timeline = first.playableAsset as TimelineAsset;
//    }

//    public void UpdateGrabPlayerKeyframe()
//    {

//        // update track for player
//        Timeline.CreateTrack<AnimationTrack>("body");
//        Timeline.CreateTrack<AnimationTrack>("hook");

//        float posx = Mathf.Ceil(_player.transform.position.x) - 0.5f;

//        AnimationTrack track = Timeline.GetOutputTrack(0) as AnimationTrack;
//        AnimationClip clip = new AnimationClip();

//        float hook_begain = 0.7f + stats.AnimationBeginTimeOffset;
//        float head_begin = 1.2f + stats.AnimationBeginTimeOffset;
//        float head_up = 1.5f + stats.AnimationBeginTimeOffset;
//        float up_keep = 2f + stats.AnimationBeginTimeOffset;
//        float up_again = 2.5f + stats.AnimationBeginTimeOffset;
//        float down_keep = 3f + stats.AnimationBeginTimeOffset;
//        float head_end = 3.5f + stats.AnimationBeginTimeOffset;
//        float hook_end = 4f + stats.AnimationBeginTimeOffset;

//        var curve_y = new AnimationCurve();
//            curve_y.AddKey(head_begin, _player.transform.position.y + 0.8f);
//            curve_y.AddKey(head_up, 2f);
//            curve_y.AddKey(up_keep, 2f);
//            curve_y.AddKey(up_again, 7f);
//            curve_y.AddKey(down_keep, 7f);
//            curve_y.AddKey(head_end, 2f);
//        //curve_y.AddKey(4f, 2f);
//        //curve_y.AddKey(4.5f, 2f);
//        var curve_x = new AnimationCurve();
//            curve_x.AddKey(up_again, posx);
//            curve_x.AddKey(down_keep, posx + stats.HookDropOffDistance);
//            clip.SetCurve("", typeof(Transform), "localPosition.y", curve_y);
//            clip.SetCurve("", typeof(Transform), "localPosition.x", curve_x);

//        TimelineClip tc = track.CreateClip(clip);
//        tc.start = 0f;
//        tc.timeScale = 1f;

//        first.SetGenericBinding(track, HeadAnimator);

//        // update track for hook
//        var track_hook = Timeline.GetOutputTrack(1) as AnimationTrack;
//        var clip_hook = new AnimationClip();

//        var curve_y_hook = new AnimationCurve();
//        curve_y_hook.AddKey(0f, 9f);
//        curve_y_hook.AddKey(hook_begain, _player.transform.position.y + 2f);
//        curve_y_hook.AddKey(head_begin, _player.transform.position.y + 2f);
//        curve_y_hook.AddKey(head_up, 3f);
//        curve_y_hook.AddKey(up_keep, 3f);
//        curve_y_hook.AddKey(up_again, 8f);
//        curve_y_hook.AddKey(down_keep, 8f);
//        curve_y_hook.AddKey(head_end, 3f);
//        curve_y_hook.AddKey(hook_end, 8f);
//        var curve_x_hook = new AnimationCurve();
//        curve_x_hook.AddKey(up_again, posx);
//        curve_x_hook.AddKey(down_keep, posx + stats.HookDropOffDistance);
//        clip_hook.SetCurve("", typeof(Transform), "localPosition.y", curve_y_hook);
//        clip_hook.SetCurve("", typeof(Transform), "localPosition.x", curve_x_hook);

//        // TODO: curves are showing auto smoothing, might need to change tangents to linear

//        TimelineClip tc_hook = track_hook.CreateClip(clip_hook);
//        tc_hook.start = 0f;
//        tc_hook.timeScale = 1f;

//        first.SetGenericBinding(track_hook, HookAnimator);

//    }

//    void CleanupTrack()
//    {
//        foreach (TrackAsset t in Timeline.GetOutputTracks())
//        {
//            Timeline.DeleteTrack(t);
//        }
//    }

//    public void PlayGrabPlayerAnimation()
//    {
//        //_player.GetComponent<PlayerController>().AlignToGrid();

//        UpdateGrabPlayerKeyframe();
//        first.Play();
//        CleanupTrack();
//    }

//    private void OnEnable()
//    {
//        first.stopped += OnPlayableDirectorStopped;
//    }

//    private void OnDisable()
//    {
//        first.stopped -= OnPlayableDirectorStopped;
//    }

//    void OnPlayableDirectorStopped(PlayableDirector aDirector)
//    {
//        if (first == aDirector)
//            OnLevelAnimationEnd.Raise();
//    }
//}
