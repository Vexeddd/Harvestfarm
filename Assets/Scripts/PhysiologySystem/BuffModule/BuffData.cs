using Newtonsoft.Json;
using System;

[Serializable]
public class BuffData
{
    public int id;
    public int item_id;

    public float duration; //持续时间

    [JsonProperty("vital_type")]
    public VitalType vitalType;

    public BuffType buffType;
    public float value;//修改值
    public float value_per_second; //每秒修改值, 针对持续性的状态
}
