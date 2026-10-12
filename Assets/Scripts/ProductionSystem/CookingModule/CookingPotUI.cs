

using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingPotUI : ProductionBuildingUI
{
    private static CookingPotUI _instance;
    public static CookingPotUI Instance
    {
        get { return _instance; }
    }

    public CookingPot currentCookingPot;

    public List<ProductionRecipe> recipes;

    public override void Awake()
    {
        base.Awake();

        _instance = this as CookingPotUI;
    }

    public override void Start()
    {
        base.Start();

        recipes = ProductionSystem.Instance.cookingModule.recipes;

        recipeItem_prefab = Resources.Load<GameObject>("Prefab/UI/RecipeItem");
        CreateRicepes(recipes);
    }


    public override void Open(ProductBuildingBase productBuildingBase)
    {
        base.Open(productBuildingBase);

    }

    public override void Close()
    {
        (currentBuilding as CookingPot).isInteracting = false;

        base.Close();
    }
}
