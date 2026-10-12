using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolModule : MonoBehaviour
{
    public ToolInstance currentTool;  //记录当前的工具

    private void OnEnable()
    {
        EventBus.Subscribe<ToolbarItemChangedEvent>(OnToolbarItemChanged);
    }

    private void Update()
    {
        if (currentTool != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UseCurrentTool();
            }
        }
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ToolbarItemChangedEvent>(OnToolbarItemChanged);
    }

    private void UseCurrentTool()
    {
        currentTool.Use();
    }

    private void OnToolbarItemChanged(ToolbarItemChangedEvent e)
    {
        if (e.inventoryItem == null)
        {
            UnEquipTool();
            return;
        }

        var itemData = e.inventoryItem.itemData;

        //如果切换的目标slot为空或者不是工具类
        if (itemData == null || itemData.item_type != ItemType.Tool)
        {
            //什么都不做

            //如果当前有工具 则卸载
            if (currentTool != null)
            {
                UnEquipTool();
            }

            return;
        }
        
        if(currentTool != null && currentTool.item != e.inventoryItem)
        {    
            UnEquipTool();
        }

        ToolData toolData = LoadManager.Instance.toolDataDic[itemData.id];
        ToolInstance ins = new ToolInstance(toolData, e.inventoryItem);
        if (ins.behaviour == null)
        {
            Debug.LogWarning($"工具 {toolData.tool_type} 尚未实现行为");
            return;
        }

        EquipTool(ins);
    }

    public void EquipTool(ToolInstance ins)
    {
        currentTool = ins;
        ins.OnEquip(this);
        print($"装备了{ins.toolData.tool_type}");
    }

    public void UnEquipTool()
    {
        print("卸载工具");
        currentTool = null;

        //卸载动画
        //卸载声音
    }



    private void OnDrawGizmos()
    {
        if (Camera.main == null || currentTool == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * currentTool.toolData.interact_distance);
    }
}
