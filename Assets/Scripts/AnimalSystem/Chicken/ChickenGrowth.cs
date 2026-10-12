using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenGrowth : MonoBehaviour
{
    public Chicken chicken;

    public float growthTimer = 0f;
    public float growDuration = 10f;

    public GameObject bigModel;
    public GameObject smallModel;

    void Awake()
    {
        var big = this.transform.FindDirect("Chicken_big");
        var small = this.transform.FindDirect("Chicken_small");
        bigModel = big != null ? big.gameObject : null;
        smallModel = small != null ? small.gameObject : null;
    }

    void Start()
    {
        StartCoroutine(Growth());
    }

    IEnumerator Growth()
    {
        if (chicken.isAdult) yield break;

        while (growthTimer < growDuration)
        {
            if (chicken.thirstLevel < 60 || chicken.hungerLevel < 60)
            {
                yield return null;
                continue;
            }

            growthTimer += 1f;
            yield return new WaitForSeconds(1);
        }

        GrowUp();
    }

    // 切到成年形态（出生即成年 / 自然长大共用）
    public void SetAdult()
    {
        chicken.isAdult = true;

        if (smallModel != null) smallModel.SetActive(false);
        if (bigModel != null) bigModel.SetActive(true);

        chicken.chickenConsume.hungerReduce = 10;
        chicken.chickenConsume.thirstReduce = 20;
    }

    private void GrowUp()
    {
        SetAdult();

        // 同步槽位物品：幼鸡 -> 成年鸡（用副本，避免污染共享模板数据）
        if (chicken.sourceItem != null)
        {
            ItemData adultData = FindAdultChickenData();
            if (adultData != null)
            {
                chicken.sourceItem.SetItem(new ItemData(adultData), 1);
            }
        }
    }

    private ItemData FindAdultChickenData()
    {
        foreach (var kv in LoadManager.Instance.itemDataDic)
        {
            if (kv.Value.item_type == ItemType.Animal && kv.Value.item_sub_type == ItemSubType.Chicken)
            {
                return kv.Value;
            }
        }
        return null;
    }
}
