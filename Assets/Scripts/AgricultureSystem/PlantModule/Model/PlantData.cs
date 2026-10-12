
using System;
using System.Collections.Generic;

/// <summary>
/// 植物数据
/// </summary>

[Serializable]
public class PlantData
{
    public int id;

    public int item_id;
    public int state_count; //总阶段
    public string name; //名称
    public string prefab_name; //预制体名称
    public List<int> timePerStage; //每阶段生长的时间
    public SeedType seed_type; //种子类型

    public int harvest_id;//收获果实的id
    public int harvest_amount; //收获果实的数量
    public string harvest_Item;

}