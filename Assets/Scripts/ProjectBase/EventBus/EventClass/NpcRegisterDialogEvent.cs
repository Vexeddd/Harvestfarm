

using System;

public class NpcRegisterDialogEvent 
{
    public DialogueNodeData data;
    public Action callBack;

    public NpcRegisterDialogEvent(DialogueNodeData data, Action callBack)
    {
        this.data = data;
        this.callBack = callBack;
    }
}
