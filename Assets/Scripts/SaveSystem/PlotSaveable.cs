using System;
using System.Collections.Generic;

public class PlotSaveable : ISaveable
{
    public void OnSave(SaveData data)
    {
        var qm = PlotSystem.Instance.questModule;

        var plot = new PlotSave
        {
            currentSegment = PlotSystem.Instance.currentSegment,
            currentNode = PlotSystem.Instance.currentRunner != null
                ? PlotSystem.Instance.currentRunner.currentIndex
                : -1,
        };

        // Save completed first, then active, so the panel shows the active quest last.
        foreach (var q in qm.completedQuests) plot.quests.Add(BuildSave(q));
        foreach (var q in qm.activeQuests) plot.quests.Add(BuildSave(q));

        data.plot = plot;
    }

    private QuestSave BuildSave(Quest q)
    {
        var qs = new QuestSave
        {
            quest_id = q.quest_id,
            title = q.title,
            isComplete = q.isComplete,
        };

        foreach (var s in q.steps)
        {
            qs.steps.Add(new StepSave
            {
                type = s.type.ToString(),
                target_id = s.target_id,
                required_count = s.required_count,
                current_count = s.current_count,
                desc = s.desc,
                isCompleted = s.isCompleted,
            });
        }

        return qs;
    }

    public void OnLoad(SaveData data)
    {
        var plot = data.plot;
        if (plot == null) return;

        var qm = PlotSystem.Instance.questModule;

        // Reset quest runtime state.
        qm.allQuests.Clear();
        qm.activeQuests.Clear();
        qm.completedQuests.Clear();

        // Rebuild quests. No grants here: items come from the inventory save, not re-granted.
        foreach (var qs in plot.quests)
        {
            var quest = BuildQuest(qs);
            qm.allQuests[quest.quest_id] = quest;

            if (quest.isComplete)
            {
                qm.completedQuests.Add(quest);
            }
            else
            {
                qm.activeQuests.Add(quest);
                QuestPanel.Instance.ShowQuest(quest);
            }
        }

        // Resume the story from where it was blocked.
        PlotSystem.Instance.currentSegment = plot.currentSegment;
        if (plot.currentNode >= 0)
        {
            PlotSystem.Instance.ResumeSegment(plot.currentSegment, plot.currentNode);
        }
    }

    private Quest BuildQuest(QuestSave qs)
    {
        var steps = new List<QuestStep>();
        foreach (var ss in qs.steps)
        {
            steps.Add(new QuestStep
            {
                type = (QuestType)Enum.Parse(typeof(QuestType), ss.type),
                target_id = ss.target_id,
                required_count = ss.required_count,
                current_count = ss.current_count,
                desc = ss.desc,
                isCompleted = ss.isCompleted,
            });
        }

        var quest = new Quest(qs.quest_id, qs.title, steps, null, null, true);
        quest.isComplete = qs.isComplete;
        return quest;
    }
}
