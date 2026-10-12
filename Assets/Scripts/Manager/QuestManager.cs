using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : SingletonAutoMono<QuestManager>
{
    protected override string RootNodeName => "[Manager]";

    public void ReportAction(QuestType questType, string targetId, int amount = 1)
    {
        PlotSystem.Instance.questModule.ReportAction(questType, targetId, amount);
    }
}
