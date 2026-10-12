using System;
using System.Collections.Generic;

/// <summary>
/// 任务类
/// </summary>
public class Quest
{
    public string quest_id;
    public string title; //任务标题
    public bool isMainQuest; //是否主线任务

    public bool isComplete; //是否完成

    public List<QuestStep> steps = new(); //任务步骤

    public List<RewardItem> grants; // items granted on accept

    public Action onComplete; //完成回调

    public Quest(string quest_id, string title, List<QuestStep> steps, Action onComplete, List<RewardItem> grants = null, bool isMainQuest = true)
    {
        this.quest_id = quest_id;
        this.title = title;
        this.isMainQuest = isMainQuest;
        this.steps = steps;
        this.onComplete = onComplete;
        this.grants = grants;
    }



    //判断任务是否全部完成
    public void CheckComplete()
    {
        foreach (var step in steps)
        {
            if (!step.isCompleted)
            {
                return;
            }
        }


        isComplete = true;
    }

}