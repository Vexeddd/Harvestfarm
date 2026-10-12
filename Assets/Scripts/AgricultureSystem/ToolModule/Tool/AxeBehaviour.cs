using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeBehaviour : BaseToolBehaviour
{
    protected override int Layer => base.Layer;
    //public ToolModule owner;
    //public void OnEquip(ToolModule toolModule, ToolInstance toolInstance)
    //{
    //    owner = toolModule;

    //    //装备的动画，声音  
    //}

    //public ToolUseResolt Use(ToolInstance toolInstance)
    //{
    //    Debug.Log("砍树");
    //    Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

    //    if (Physics.Raycast(ray, out RaycastHit hit, toolInstance.toolData.interact_distance, LayerMask.GetMask("Tree")))
    //    {
    //        if (hit.collider.TryGetComponent(out IToolInteractable target))
    //        {
    //            //Debug.Log("检测到树");
    //            ToolContext context = new ToolContext(toolInstance.toolData, owner.gameObject, hit.point);

    //            ToolUseResolt res = target.OnToolUsed(context);
    //            return res;
    //        }
    //        else
    //        {
    //            return ToolUseResolt.Invalid;
    //        }
    //    }
    //    else
    //    {
    //        return ToolUseResolt.None;
    //    }
    //}
}
