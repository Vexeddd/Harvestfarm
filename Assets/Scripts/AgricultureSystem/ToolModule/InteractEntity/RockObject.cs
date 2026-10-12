using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockObject : MonoBehaviour, IToolInteractable
{
    private int Hp = 3;
    public ToolUseResolt OnToolUsed(ToolContext context)
    {
        if (context.toolData.tool_type != ToolType.Pickaxe)
        {
            return ToolUseResolt.Invalid;
        }

        Hp--;

        if (Hp <= 0)
        {
            ItemData item = new ItemData(LoadManager.Instance.itemDataDic[100]);
            item.current_stack = 3;
            InventorySystem.Instance.CreatItemInWorld(item, this.transform.position);

            Destroy(this.gameObject);
        }

        return ToolUseResolt.Success;
    }
}
