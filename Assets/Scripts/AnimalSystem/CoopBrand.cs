using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopBrand : MonoBehaviour, IInteractalbe
{
    public bool isInteracting = false;

    public ChickenCoop chickenCoop;

    public bool CanInteract()
    {
        return !isInteracting;
    }

    public string GetInteractTip()
    {
        return "按F 查看鸡舍信息";
    }

    public void Ineract(GameObject obj)
    {
        ChickenCoopUI.Instance.Open(chickenCoop);
        SetInteractable(false);
        //InteractionUI.Instance.Hide();
    }


    public void SetInteractable(bool can)
    {
        isInteracting = !can;
    }

    void Update()
    {
        if (!isInteracting)
        {
            return;
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChickenCoopUI.Instance.Close();
            }
        }
    }
}
