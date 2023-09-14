using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private PlayableDirector end_director;

    AsyncOperation operation;

    private void OnEnable()
    {
        end_director.stopped += OnPlayableDirectorStopped;
    }

    private void OnDisable()
    {
        end_director.stopped -= OnPlayableDirectorStopped;
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (end_director == aDirector)
            operation.allowSceneActivation = true;
    }

    public void BackToTitle()
    {
        StartCoroutine(LoadSceneAsync());
        end_director.Play();
    }

    IEnumerator LoadSceneAsync()
    {
        operation = SceneManager.LoadSceneAsync("Title");
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            //float progress = Mathf.Clamp01(operation.progress / 0.99f);

            //fill.fillAmount = progress;

            yield return null;
        }
    }
}
