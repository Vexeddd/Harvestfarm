using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChickenCoop : MonoBehaviour
{
    public List<Chicken> chickens = new List<Chicken>();//小鸡引用

    public Collider activityScope;//可活动范围（触发器）

    public CoopFoodBucket coopFoodBucket;//食物

    public CoopWaterBucket coopWaterBucket;//水槽

    public CoopBrand coopBrand;//广告牌 


    void Awake()
    {
        coopBrand = this.transform.FindDirect("CoopBrand").GetComponent<CoopBrand>();
        activityScope = this.transform.FindDirect("ChickenActivityScope").GetComponent<Collider>();

        coopBrand.chickenCoop = this;
    }

    void Start()
    {
        coopFoodBucket = this.transform.FindRecursive("CoopFoodBucket").AddComponent<CoopFoodBucket>();
        coopFoodBucket.Init(this);

        coopWaterBucket = this.transform.FindRecursive("CoopWaterBucket").AddComponent<CoopWaterBucket>();
        coopWaterBucket.Init(this);
    }

    public Chicken AddChicken(InventoryItem sourceItem = null)
    {
        bool isAdult = sourceItem != null && sourceItem.itemData.item_sub_type == ItemSubType.Chicken;

        Vector3 pos = GetRandomPointInArea();
        GameObject chicken_Prefab = AnimalSystem.Instance.chicken_Prefab;

        GameObject go = GameObject.Instantiate(chicken_Prefab, pos, Quaternion.identity);
        Chicken chicken = go.GetComponent<Chicken>();
        if (chicken == null)
        {
            chicken = go.AddComponent<Chicken>();
        }

        chickens.Add(chicken);

        chicken.Init(this, isAdult, sourceItem);

        return chicken;
    }

    public void RemoveChicken(Chicken chicken)
    {
        if (chicken == null) return;
        chickens.Remove(chicken);
        Destroy(chicken.gameObject);
    }

    public Vector3 GetRandomPointInArea()
    {
        Vector3 center = activityScope.bounds.center;
        Vector3 size = activityScope.bounds.size;

        return new Vector3(
            center.x + UnityEngine.Random.Range(-size.x / 2, size.x / 2),
            0,
            center.z + UnityEngine.Random.Range(-size.z / 2, size.z / 2)
            );
    }
}
