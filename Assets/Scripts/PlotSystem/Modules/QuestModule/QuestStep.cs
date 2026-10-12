using System;
using UnityEngine;

/// <summary>
/// 任务中的小步骤
/// </summary>
[Serializable]
public class QuestStep
{
    public QuestType type; // 目标任务类型
    public string target_id; //目标id
    public int required_count; // 需要的数量
    public int current_count; // 当前完成的数量

    public string desc; //描述

    public bool isCompleted; //是否完成

    public Action OnProgress;
    public Action OnCompleted; //完成回调


    //尝试更新进度
    public void TryProgress(QuestType actionType, string actionTarget, int amount)
    {
        if (isCompleted)
        {
            Debug.Log("当前任务已经完成");
            return;
        }

        //不是需要的类型, 需要杀怪的 但传过来是种植的
        if (type != actionType)
        {
            Debug.Log("当前任务不是需要的类型");
            return;
        }


        //比对目标id
        if (target_id != actionTarget)
        {
            Debug.Log("当前任务目标id不相等");
            return;
        }


        //增加进度
        current_count += amount;
        OnProgress?.Invoke();
        Debug.Log("当前进度" + current_count);

        if (current_count >= required_count)
        {
            current_count = required_count;
            isCompleted = true; //标记为真
            OnCompleted?.Invoke(); //调用回调
        }
    }
}