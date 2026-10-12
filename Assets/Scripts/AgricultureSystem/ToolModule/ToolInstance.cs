using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToolInstance
{
    public ToolData toolData;
    public InventoryItem item;
    public IBehaviour behaviour;

    public ToolRuntimeState toolState;

    public ToolInstance(ToolData toolData, InventoryItem item)
    {
        this.toolData = toolData;
        this.item = item;
        behaviour = ToolBehaviorFactory.CreateBehaviour(toolData.tool_type);

        if (StateManager.GetState<ToolRuntimeState>(this.item.itemData.uuid) is null)
        {
            toolState = new ToolRuntimeState(this.item.itemData.uuid);
            toolState.current_duration = toolData.max_durability;

            if (toolData.tool_type == ToolType.WaterCan)
            {
                toolState.current_water = 10;
            }

            StateManager.RegisterState(this.item.itemData.uuid, toolState);
        }
        else
        {
            toolState = StateManager.GetState<ToolRuntimeState>(this.item.itemData.uuid);
        }
    }

    public void OnEquip(ToolModule toolModule)
    {
        behaviour.OnEquip(toolModule, this);
    }


    public void Use()
    {
        //使用前判断

        //通用的判断
        //是否有体力 -> 体力系统 -> 响应式变量
        float currentStamina = 0f;
        if (toolData.use_stamina)
        {
            //查询体力
            EventBus.Publish<VitalQueryRequestEvent>(new VitalQueryRequestEvent
            {
                type = VitalType.Stamina,
                callBack = (val) =>
                {
                    currentStamina = val;
                }
            });

            if (toolData.stamina_cost > currentStamina)
            {
                return;
            }
        }



        ToolUseResolt useResult;

        if (toolData.tool_type != ToolType.WaterCan)
        {
            useResult = behaviour.Use(this);
        }
        else
        {
            if (toolState.current_water < 1)
            {
                useResult = ToolUseResolt.Invalid;
            }
            else
            {
                useResult = behaviour.Use(this);
            }
        }





        if (useResult == ToolUseResolt.Success || useResult == ToolUseResolt.Invalid)
        {
            if (toolData.use_stamina)
            {
                CommandResult res = CommandBus.Send(new CommandConsumeStamina { amount = toolData.stamina_cost });
            }
        }

        //是否有耐久 -> 状态管理器
        if (useResult == ToolUseResolt.Success)
        {
            Debug.Log($"使用工具，消耗耐久{toolData.durability_cost}，剩余耐久{toolState.current_duration - toolData.durability_cost}");
            if (IsBroken())
            {
                item.SetItemAmount(0);
                AgricultureSystem.Instance.toolModule.UnEquipTool();
            }
            else
            {
                toolState.current_duration -= toolData.durability_cost;
            }
        }


    }

    public bool IsBroken()
    {
        if (!toolData.use_durability)
        {
            return false;
        }

        if (toolState.current_duration > toolData.durability_cost)
        {
            return false;
        }

        return true;
    }

    public void FillWater()
    {
        if (toolData.tool_type == ToolType.WaterCan)
        {
            toolState.current_water = 10;

            //播放音效
            MusicManager.Instance.PlayOneShot(MusicType.FillWaterCan);
        }
    }
}
