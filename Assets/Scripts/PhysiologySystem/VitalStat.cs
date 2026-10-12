using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VitalStat
{
    public Reactive<float> Current = new Reactive<float>();
    public float Max;

    public VitalStat(float max)
    {
        Max = max;
        Current.Value = max;
    }

    public bool ModifyValue(float val)
    {
        if (val < 0 && Current.Value + val < 0)
        {
            Current.Value = 0;
            return false;
        }
        Current.Value = Mathf.Clamp(Current.Value + val, 0, Max);
        return true;
    }

    public void SetValue(float val)
    {
        Current.Value = val;
    }

    public float Percent => Current.Value / Max;
}

