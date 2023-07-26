using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private LevelStats stats;
    [SerializeField] private TMP_Text stepCount;
    [SerializeField] private TMP_Text levelCount;
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
    }

    void UpdateLevelDisplay(int level)
    {
        levelCount.text = "Level " + stats.Level.ToString();
    }
}
