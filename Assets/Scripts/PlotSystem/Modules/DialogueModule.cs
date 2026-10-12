using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueModule : MonoBehaviour
{
    public void RegisterDialogue(DialogueNodeData data, Action callBack)
    {
        EventBus.Publish(new NpcRegisterDialogEvent(data, callBack));
    }
}
