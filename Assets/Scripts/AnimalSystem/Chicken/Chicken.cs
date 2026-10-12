using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    public ChickenCoop chickenCoop;
    public Vector3 activityScope;

    public float hungerLevel;
    public float thirstLevel;

    public ChickenConsume chickenConsume;
    public ChickenGrowth chickenGrowth;

    public bool isAdult = false;

    // 与鸡舍槽位里的物品绑定：长大后同步物品类型 / 取出时定位世界里的鸡
    public InventoryItem sourceItem;

    public void Init(ChickenCoop coop, bool adult = false, InventoryItem item = null)
    {
        this.chickenCoop = coop;
        this.activityScope = chickenCoop.GetRandomPointInArea();
        this.isAdult = adult;
        this.sourceItem = item;

        hungerLevel = 100;
        thirstLevel = 100;

        this.AddComponent<ChickenWander>().chicken = this;

        chickenConsume = this.AddComponent<ChickenConsume>();
        chickenConsume.chicken = this;

        chickenGrowth = this.AddComponent<ChickenGrowth>();
        chickenGrowth.chicken = this;

        this.AddComponent<ChickenEggLayer>().chicken = this;

        // 成年鸡直接以成年形态出生（成长协程对成年鸡会直接跳过）
        if (isAdult)
        {
            chickenGrowth.SetAdult();
        }
    }
}
