using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, IInteractalbe
{
    public NpcData npcData;

    public bool isInteracting;

    public void Init(NpcData npcData)
    {
        this.npcData = npcData;
    }


    public bool CanInteract()
    {
        return !isInteracting;
    }

    public string GetInteractTip()
    {
        return $"°´FºÍ {npcData.name} »¥¶¯";
    }

    public void Ineract(GameObject obj)
    {
        QuestManager.Instance.ReportAction(QuestType.TalkToNPC, npcData.npcId);

        NPCInteractPanel.Instance.Open(this);
        isInteracting = true;   
    }

}
