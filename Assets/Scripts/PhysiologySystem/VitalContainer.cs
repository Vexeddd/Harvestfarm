using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalContainer
{
    public Dictionary<VitalType, VitalStat> _vitals = new Dictionary<VitalType, VitalStat>();

    public VitalStat Register(VitalType type, float max)
    {
        VitalStat state;
        if (_vitals.TryGetValue(type, out state))
        {
            state.Max = max;
            return state;
        }

        state = new VitalStat(max);
        _vitals.Add(type, state);
        return state;
    }

    public VitalStat Get(VitalType type)
    {
        return _vitals[type];
    }
}
