using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ToolBehaviorFactory 
{
    public static IBehaviour CreateBehaviour(ToolType type)
    {
        switch (type)
        {
            case ToolType.Hoe:
                return new HoeBehaviour();
            case ToolType.Axe:
                return new AxeBehaviour();
            case ToolType.Pickaxe:
                return new PickaxeBehaviour();
            case ToolType.WaterCan:
                return new WaterCanBehaviour();
            default:
                return null;
        }
    }
}
