
using System;

public class VitalQueryRequestEvent
{
    public VitalType type;

    public Action<float> callBack;

}
