using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestStepItemUI : MonoBehaviour
{
    public QuestStep currentStep;

    public TMP_Text stepText;

    private void Awake()
    {
        if (stepText == null)
        {
            stepText = transform.FindRecursive("StepText").GetComponent<TMP_Text>();
        }
    }

    public void Bind(QuestStep step)
    {
        currentStep = step;

        currentStep.OnProgress += OnStepProgress;
        currentStep.OnCompleted += OnStepCompleted;
        Refresh();
    }


    private void OnStepProgress()
    {
        Refresh();
    }


    private void OnStepCompleted()
    {
        Refresh();
    }


    private void Refresh()
    {
        stepText.text = $"{currentStep.desc}   {currentStep.current_count}/{currentStep.required_count}";
        //如果任务完成
        if (currentStep.isCompleted)
        {
            stepText.fontStyle |= FontStyles.Strikethrough;

            Color c = stepText.color;
            c.a = 0.4f;
            stepText.color = c;
        }
    }

    private void OnDestroy()
    {
        currentStep.OnProgress -= OnStepProgress;
        currentStep.OnCompleted -= OnStepCompleted;
    }

}
