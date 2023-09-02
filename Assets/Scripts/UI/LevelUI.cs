using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private LevelStats stats;
    [SerializeField] private TMP_Text stepCount;
    [SerializeField] private TMP_Text levelCount;
    [SerializeField] private Image bucketFill;
    [SerializeField] private GameObject bucket;
    [SerializeField] private float fillSpeed;
    void Start()
    {
        if (stats.Level > 0)
        {
            stepCount.text = stats.StepsLeft.ToString();
            levelCount.text = "Level " + stats.Level.ToString();
        }
    }

    private void OnEnable()
    {

        stats.StepsLeftChanged += UpdateStepDisplay;
        stats.LevelChanged += UpdateLevelDisplay;
        // level passed in here as well
        stats.LevelChanged += UpdateStepDisplay;
        
    }

    private void OnDisable()
    {

        stats.StepsLeftChanged -= UpdateStepDisplay;
        stats.LevelChanged -= UpdateLevelDisplay;
        stats.LevelChanged -= UpdateStepDisplay;
        
    }

    void UpdateStepDisplay(int step)
    {
        stepCount.text = stats.StepsLeft.ToString();
        UpdateFillAmount();
    }

    void UpdateLevelDisplay(int level)
    {
        levelCount.text = "Level " + level.ToString();
    }

    public void UpdateFillAmount()
    {
        float percent = (float)stats.StepsLeft / stats.LevelSteps[stats.Level];
        float current_fill = bucketFill.fillAmount;
        FillAnimation(current_fill, percent);
    }

    public void SetFillAmount(float percent)
    {
        bucketFill.fillAmount = percent;
    }

    public void FillAnimation(float from, float to)
    {
        Debug.Log(bucketFill.fillAmount + "  percent " + to);
        LeanTween.value(bucketFill.gameObject, SetFillAmount, from, to, 0.5f);
        Vector2 pos = bucket.GetComponent<RectTransform>().anchoredPosition;
        LeanTween.move(bucket.GetComponent<RectTransform>(), new Vector2(pos.x-2f, pos.y), 0.2f).setEaseShake();
    }
}
