using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToolInteractable
{
    ToolUseResolt OnToolUsed(ToolContext context);
}

public class ToolContext
{
    public ToolData toolData; //工具数据
    public int toolId; //工具id
    public GameObject user; //使用者
    public Vector3 hitPoint;  //命中点

    public ToolContext(ToolData toolData, GameObject user, Vector3 hitPoint)
    {
        this.toolData = toolData;
        this.toolId = toolData.tool_id;
        this.user = user;
        this.hitPoint = hitPoint;
    }
}

public enum ToolUseResolt
{
    None,
    Invalid,
    Success
}