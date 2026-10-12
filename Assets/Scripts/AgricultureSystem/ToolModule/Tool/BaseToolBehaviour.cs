using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseToolBehaviour : IBehaviour
{
    protected virtual int Layer => LayerMask.GetMask("Tree");

    public ToolModule owner;
    public virtual void OnEquip(ToolModule toolModule, ToolInstance toolInstance)
    {
        owner = toolModule;
    }

    public virtual ToolUseResolt Use(ToolInstance toolInstance)
    {
        Debug.Log("สนำร" + toolInstance.toolData.tool_type.ToString());
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, toolInstance.toolData.interact_distance, Layer))
        {
            if (hit.collider.TryGetComponent(out IToolInteractable target))
            {
                ToolContext context = new ToolContext(toolInstance.toolData, owner.gameObject, hit.point);

                ToolUseResolt res = target.OnToolUsed(context);
                return res;
            }
            else
            {
                return ToolUseResolt.Invalid;
            }
        }
        else
        {
            return ToolUseResolt.None;
        }
    }
}
