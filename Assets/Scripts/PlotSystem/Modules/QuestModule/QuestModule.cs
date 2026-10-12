using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestModule : MonoBehaviour
{
    public Dictionary<string, Quest> allQuests = new(); //所有任务字典
    public List<Quest> activeQuests = new(); //当前正在进行的任务
    public List<Quest> completedQuests = new(); //已完成的任务

    public void RegisterQuest(Quest quest)
    {
        allQuests[quest.quest_id] = quest;
        activeQuests.Add(quest);

        GrantItems(quest);

        QuestPanel.Instance.ShowQuest(quest);
    }

    private void GrantItems(Quest quest)
    {
        if (quest.grants == null) return;
        foreach (var reward in quest.grants)
        {
            InventorySystem.Instance.TryAddItem(reward.item_id, reward.count);
        }
    }

    public void ReportAction(QuestType questType, string targetId, int amount = 1)
    {
        for (int i = 0; i < activeQuests.Count; i++)
        {
            Quest quest = activeQuests[i];
            foreach (var step in quest.steps)
            {
                step.TryProgress(questType, targetId, amount);
            }

            quest.CheckComplete();
            if (quest.isComplete)
            {
                activeQuests.RemoveAt(i);
                completedQuests.Add(quest);

                OnQuestCompleted(quest);
            }
        }
    }

    private void OnQuestCompleted(Quest quest)
    {
        quest.onComplete?.Invoke();

        //发放奖励什么的
    }
}
