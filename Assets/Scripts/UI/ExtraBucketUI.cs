using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ExtraBucketUI : MonoBehaviour
{
    [SerializeField] private LevelStats stats;
    [SerializeField] private TMP_Text stepCount;
    [SerializeField] private Image bucketFill;
    [SerializeField] private Canvas extra_bucket;
    void Start()
    {

        
    }

    private void OnEnable()
    {
        stats.ExtraStepsLeftChanged += UpdateExtraStepDisplay;
        
    }

    private void OnDisable()
    {

        stats.ExtraStepsLeftChanged -= UpdateExtraStepDisplay;
        
    }

    public void UpdateExtraStepDisplay(int step)
    {
        if (extra_bucket != null)
        {


            if (stats.ExtraStepLeftOnBegin > 0)
            {
                extra_bucket.enabled = true;
                stepCount.text = stats.ExtraStepsLeft.ToString();
                UpdateExtraFillAmount();
            }
            else
            {
                extra_bucket.enabled = false;
            }
        }
    }

    public void SetFillAmount(float percent)
    {
        bucketFill.fillAmount = percent;
    }

    public void UpdateExtraFillAmount()
    {
        if (stats.ExtraStepLeftOnBegin > 0)
        {
            float percent = (float)stats.ExtraStepsLeft / stats.ExtraStepLeftOnBegin;
            float current_fill = bucketFill.fillAmount;
            FillAnimation(current_fill, percent);
        }
        
    }

    public void SetExtraFillAmount(float percent)
    {
        bucketFill.fillAmount = percent;
    }

    public void FillAnimation(float from, float to)
    {
        //Debug.Log(bucketFill.fillAmount + "  percent " + to);
        LeanTween.value(bucketFill.gameObject, SetFillAmount, from, to, 0.5f);
        Vector2 pos = extra_bucket.GetComponent<RectTransform>().anchoredPosition;
        LeanTween.move(extra_bucket.GetComponent<RectTransform>(), new Vector2(pos.x-2f, pos.y), 0.2f).setEaseShake();
    }
}
