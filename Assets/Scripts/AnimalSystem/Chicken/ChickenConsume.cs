using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenConsume : MonoBehaviour
{
    public Chicken chicken;

    private float consumeInterval = 3f;
    private float consumeTimer = 0;

    private float foodConsume = 2f;
    private float waterConsume = 2f;

    public float hungerReduce = 5;
    public float thirstReduce = 10;

    void Update()
    {
        UpdateConsume();
        UpdateSupplyment();
    }

    private void UpdateSupplyment()
    {
        if (chicken.hungerLevel < 60)
        {
            bool eatFood = chicken.chickenCoop.coopFoodBucket.ConsumeFood(foodConsume);
            if (eatFood)
            {
                chicken.hungerLevel += 30;
            }
        }

        if (chicken.thirstLevel < 60)
        {
            bool drinkWater = chicken.chickenCoop.coopWaterBucket.ConsumeWater(waterConsume);
            if (drinkWater)
            {
                chicken.thirstLevel += 30;
            }
        }
    }

    private void UpdateConsume()
    {
        consumeTimer += Time.deltaTime;
        if (consumeTimer >= consumeInterval)
        {
            consumeTimer = 0;

            chicken.hungerLevel = Mathf.Max(chicken.hungerLevel - 5, 0);
            chicken.thirstLevel = Mathf.Max(chicken.thirstLevel - 10, 0);
        }
    }
}
