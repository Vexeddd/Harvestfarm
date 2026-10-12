using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveQuestNode : IStoryNode
{
    public GiveQuestNodeData data;

    public GiveQuestNode() { }

    public GiveQuestNode(GiveQuestNodeData data)
    {
        this.data = data;
    }

    public void StartNode(Action<object> callBack)
    {
        var questModule = PlotSystem.Instance.questModule;

        if (questModule.allQuests.TryGetValue(data.quest_id, out var existing))
        {
            if (existing.isComplete)
            {
                callBack?.Invoke(null);
            }
            else
            {
                existing.onComplete += () => callBack?.Invoke(null);
            }
            return;
        }


        //¹¹½¨Quest
        Quest quest = new Quest
            (
            data.quest_id,
            data.title,
            data.steps,
            () =>
            {
                callBack?.Invoke(null);
            },
            data.grants
            );

        PlotSystem.Instance.questModule.RegisterQuest(quest);
    }
    
    public static IStoryNode CreateFromJson(INodeBody body)
    {
        return new GiveQuestNode(body as GiveQuestNodeData);
    }
}
