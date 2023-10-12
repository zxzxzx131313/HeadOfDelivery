using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dissolve : MonoBehaviour
{
    [SerializeField]
    private float _dissolveTime = 2f;

    private Material mat;
    private int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    void Start()
    {
        //mat = Instantiate(GetComponent<Image>().material);
        //GetComponent<Image>().material = mat;
        mat = GetComponent<Image>().material;
        mat.SetFloat(_dissolveAmount, 0f);
    }

    public void Vanish()
    {
        StartCoroutine(Dissolving());
    }

    public void VanishCanvas(CanvasGroup canvas)
    {
        StartCoroutine(DissolvingCanvas(canvas));
    }

    IEnumerator Dissolving()
    {
        float elapsTime = 0f;
        while (elapsTime < _dissolveTime)
        {
            elapsTime += Time.deltaTime;

            float lerpDissolve = Mathf.Lerp(0, 1.1f, (elapsTime / _dissolveTime));

            mat.SetFloat(_dissolveAmount, lerpDissolve);
            yield return null;
        }
    }

    IEnumerator DissolvingCanvas(CanvasGroup canvas)
    {
        float elapsTime = 0f;
        while (elapsTime < _dissolveTime)
        {
            elapsTime += Time.deltaTime;

            float lerpDissolve = Mathf.Lerp(1.1f, 0f, (elapsTime / _dissolveTime));

            canvas.alpha = lerpDissolve;
            yield return null;
        }
    }
}
