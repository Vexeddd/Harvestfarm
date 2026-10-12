using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    private static QuestPanel _instance;
    public static QuestPanel Instance
    {
        get { return _instance; }
    }


    public TMP_Text titleText;
    public Transform stepContainer;

    public Quest currentQuest;

    private void Awake()
    {
        _instance = this;

        titleText = transform.FindRecursive("TitleText").GetComponent<TMP_Text>();
        stepContainer = transform.FindRecursive("StepContainer");
    }


    public void ShowQuest(Quest quest)
    {
        Clear();

        currentQuest = quest;

        titleText.text = currentQuest.title;

        foreach (var step in currentQuest.steps)
        {
            QuestStepItemUI item = Instantiate(PlotSystem.Instance.stepItemPrefab, stepContainer).GetComponent<QuestStepItemUI>();
            item.Bind(step);
        }

        quest.onComplete += OnQuestCompleted;
    }

    private void OnQuestCompleted()
    {
        //²¥·ÅÌØÐ§
    }

    private void Clear()
    {
        for (int i = stepContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(stepContainer.GetChild(i).gameObject);
        }

        titleText.text = "";

        currentQuest = null;
    }
}
