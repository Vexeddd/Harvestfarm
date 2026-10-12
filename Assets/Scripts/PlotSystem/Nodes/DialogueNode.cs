using System;
using UnityEngine;

public class DialogueNode : IStoryNode
{
    public DialogueNodeData data;

    public DialogueNode() { }

    public DialogueNode(DialogueNodeData data)
    {
        this.data = data;
    }


    public void StartNode(Action<object> callBack)
    {
        PlotSystem.Instance.dialogueModule.RegisterDialogue(data, () => 
        {
            callBack?.Invoke(null);
        });
    }

    public static IStoryNode CreateFromJson(INodeBody body)
    {
        return new DialogueNode(body as DialogueNodeData);
    }
}
