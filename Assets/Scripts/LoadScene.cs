using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private PlayableDirector end_director;
    [SerializeField] private GameObject end_veil;

    AsyncOperation operation;

    private void Start()
    {

    }

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
        StartCoroutine(LoadSceneAsync(0));
        end_director.Play();
    }

    public void ToLevel(int id)
    {
        StartCoroutine(LoadSceneAsync(id));
        end_director.Play();
    }

    public void ToNextLevel()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        ToLevel(next);
    }

    IEnumerator LoadSceneAsync(int id)
    {
        operation = SceneManager.LoadSceneAsync(id);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            //float progress = Mathf.Clamp01(operation.progress / 0.99f);

            //fill.fillAmount = progress;

            yield return null;
        }
    }


}
