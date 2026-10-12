using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineNode : IStoryNode
{
    public TimelineNodeData data;

    public TimelineNode()  { }

    public TimelineNode(TimelineNodeData data)
    {
        this.data = data;
    }


    public void StartNode(Action<object> callBack)
    {
        
    }


    public static IStoryNode CreateFromJson(INodeBody body)
    {
        return new TimelineNode(body as TimelineNodeData);
    }
}
