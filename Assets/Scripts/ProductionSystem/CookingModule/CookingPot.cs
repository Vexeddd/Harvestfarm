using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingPot : ProductBuildingBase
{
    public bool isInteracting = false;

    private void Awake()
    {
        //初始化数据
        InitInputs(4);
        InitOutputs(1);
        InitFuels(2);
    }

    public override bool CanInteract()
    {
        return !isInteracting;
    }
    public override void Ineract(GameObject obj)
    {
        //base.Ineract(obj);


        CookingPotUI.Instance.Open(this);
        isInteracting = true;

    }

    public override void Update()
    {
        base.Update();

        if (!isInteracting)
        {
            return;
        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CookingPotUI.Instance.Close();
            }
        }
    }
}
