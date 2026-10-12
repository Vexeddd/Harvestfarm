using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWall : MonoBehaviour, IInteractalbe
{
    public bool CanInteract()
    {
        var currentTool = AgricultureSystem.Instance.toolModule.currentTool;

        if (currentTool != null && AgricultureSystem.Instance.toolModule.currentTool.toolData.tool_type == ToolType.WaterCan)
        {
            return true;
        }

        return false;
    }

    public string GetInteractTip()
    {
        return "°´F ¹àÂúË®ºø";
    }

    public void Ineract(GameObject obj)
    {
        AgricultureSystem.Instance.toolModule.currentTool.FillWater();
    }
}
