using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCanBehaviour : BaseToolBehaviour
{
    protected override int Layer => LayerMask.GetMask("Farmland", "WaterCanAbout");


    public override ToolUseResolt Use(ToolInstance toolInstance)
    {
        Debug.Log("使用" + toolInstance.toolData.tool_type.ToString());
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, toolInstance.toolData.interact_distance, Layer))
        {
            if (hit.collider.TryGetComponent(out IToolInteractable target))
            {
                ToolContext context = new ToolContext(toolInstance.toolData, owner.gameObject, hit.point);
                toolInstance.toolState.current_water -= 1;
                Debug.Log($"使用了水壶，当前水量：{toolInstance.toolState.current_water}");

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
