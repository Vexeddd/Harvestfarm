using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopFoodBucket : MonoBehaviour, IInteractalbe
{
    public ChickenCoop chickenCoop;

    public float currentFood;
    public float maxFood = 100;

    public void Init(ChickenCoop coop)
    {
        this.chickenCoop = coop;
        this.currentFood = 0;
    }

    public bool AddFood(float amount)
    {
        if (currentFood >= maxFood)
        {
            return false;
        }

        currentFood = Mathf.Min(currentFood + amount, maxFood);
        return true;
    }

    public bool ConsumeFood(float amount)
    {
        bool enoughFood = EnoughFood(amount);
        if (!enoughFood)
        {
            return false;
        }

        currentFood -= amount;
        return true;
    }

    public bool EnoughFood(float amount)
    {
        return currentFood >= amount;
    }

    public bool CanInteract()
    {
        return true;
    }

    public string GetInteractTip()
    {
        return "按F 添加食物";
    }

    public void Ineract(GameObject obj)
    {
        AddFood(10);
    }

}
