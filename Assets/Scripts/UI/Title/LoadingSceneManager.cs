using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Cinemachine;

public class LoadingSceneManager : MonoBehaviour
{

    [SerializeField] private GameObject Loading;
    //public Image fill;
    [SerializeField] private PlayableDirector End;
    [SerializeField] private PlayableDirector Loop;

    CinemachineVirtualCamera vcam;
    bool played = false;
    bool loop_played = false;
    AsyncOperation operation;

    private void Start()
    {
        Loading.SetActive(false);

        var camera = Camera.main;
        var brain = (camera == null) ? null : camera.GetComponent<CinemachineBrain>();
        vcam = (brain == null) ? null : brain.ActiveVirtualCamera as CinemachineVirtualCamera;
    }

    private void OnEnable()
    {
        End.stopped += OnPlayableDirectorStopped;
        Loop.stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        End.stopped -= OnPlayableDirectorStopped;
        Loop.stopped -= OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {

        if (End == aDirector)
            operation.allowSceneActivation = true;

        if (Loop == aDirector)
            loop_played = true;

        Debug.Log(aDirector.name);
    }

    void PlayAnimation()
    {
        End.Play();
    }

    public void LoadScene(int id)
    {
        //ZoomIn();
        Loop.extrapolationMode = DirectorWrapMode.None;

        Loading.SetActive(true);
        StartCoroutine(LoadSceneAsync(id));

        PlayAnimation();
    }

    IEnumerator LoadSceneAsync(int id)
    {
        operation = SceneManager.LoadSceneAsync(id);
        operation.allowSceneActivation = false;

        while (!operation.isDone && !loop_played)
        {
            //float progress = Mathf.Clamp01(operation.progress / 0.99f);

            //fill.fillAmount = progress;

            yield return null;
        }
    }

    void SetOrtho(float value)
    {
        vcam.m_Lens.OrthographicSize = value;
    }

    void ZoomIn()
    {
        float from = vcam.m_Lens.OrthographicSize;
        float pos_from = vcam.transform.position.x;
        if (vcam != null)
        {
            LeanTween.value(vcam.gameObject, SetOrtho, from, from - 1f, 1f);
            LeanTween.moveX(vcam.gameObject, pos_from - 1.3f, 1f);
        }
    }
}
