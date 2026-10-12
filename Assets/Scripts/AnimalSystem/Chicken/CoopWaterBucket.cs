using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopWaterBucket : MonoBehaviour, IToolInteractable
{
    public float currentWater;
    public float maxWater = 100;

    public ChickenCoop chickenCoop;

    public void Init(ChickenCoop coop)
    {
        this.chickenCoop = coop;
        currentWater = 0f;
    }

    public bool AddWater(float amount)
    {
        if (currentWater >= maxWater)
        {
            return false;
        }

        currentWater = Mathf.Min(currentWater + amount, maxWater);
        return true;
    }

    public bool ConsumeWater(float amount)
    {
        bool enoughWater = EnoughWater(amount);
        if (!enoughWater)
        {
            return false;
        }

        currentWater -= amount;
        return true;
    }

    public bool EnoughWater(float amount)
    {
        return currentWater >= amount;
    }

    public ToolUseResolt OnToolUsed(ToolContext context)
    {
        if (context.toolData.tool_type != ToolType.WaterCan)
        {
            return ToolUseResolt.Invalid;
        }

        bool res = AddWater(10);

        if (!res)
        {
            return ToolUseResolt.None;
        }

        return ToolUseResolt.Success;
    }
}
