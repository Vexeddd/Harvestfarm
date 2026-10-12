using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class ItemData
{
    public ItemData()
    {

    }

    public ItemData(ItemData itemData)
    {
        this.id = itemData.id;
        this.uuid = itemData.uuid;
        this.name = itemData.name;
        this.describe = itemData.describe;
        this.icon_name = itemData.icon_name;
        this.item_type = itemData.item_type;
        this.item_sub_type = itemData.item_sub_type;

        this.has_state = itemData.has_state;
        this.is_stackable = itemData.is_stackable;
        this.max_stack = itemData.max_stack;
        this.current_stack = itemData.current_stack;
        this.is_durable = itemData.is_durable;
        this.max_durable = itemData.max_durable;
        this.current_durable = itemData.current_durable;

        this.icon = itemData.icon;
    }

    public int id; //物品id  1
    public string uuid;
    public string name; //物品名称 鸡蛋 
    public string describe; //物品详细描述 可以吃
    public string icon_name; //图标名称
    public float price; //价格
    public ItemType item_type;
    public ItemSubType item_sub_type; //物品类型二级细分

    //堆叠相关
    public bool is_stackable; //是否可以堆叠
    public int max_stack; //最大可以堆叠数量
    public int current_stack; //当前的数量

    //耐久相关
    public bool has_state;
    public bool is_durable; //是否有耐久
    public int max_durable; //最大耐久
    public int current_durable; //当前耐久度

    //图像
    [JsonIgnore]
    public Sprite icon;

}