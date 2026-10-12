using System;
using System.Collections.Generic;

public static class StateManager
{
    private static Dictionary<string, ItemRuntimeState> stateDic = new Dictionary<string, ItemRuntimeState>();

    //生成UUID的方法
    public static string GenerateUUID()
    {
        return Guid.NewGuid().ToString("N");
    }

    public static ItemRuntimeState RegisterState(string uuid, ItemRuntimeState state)
    {
        state.uuid = uuid;

        if (stateDic.TryGetValue(uuid, out var itemstate))
        {
            stateDic[uuid] = state;

            return state;
        }

        stateDic.Add(uuid, state);

        return state;
    }

    public static T GetState<T>(string uuid) where T : ItemRuntimeState
    {
        if (string.IsNullOrEmpty(uuid))
        {
            return null;
        }

        if (stateDic.TryGetValue(uuid, out var state))
        {
            return state as T;
        }

        return null;
    }
}
