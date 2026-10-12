using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeItemUI : MonoBehaviour, IPointerClickHandler
{
    public ProductionRecipe recipe;

    public ProductionBuildingUI buildUI;

    public bool IsSelected;

    public Image _backGround;
    public Image _icon;


    private void Awake()
    {
        _backGround = this.transform.FindRecursive("Background").GetComponent<Image>();
        _icon = this.transform.FindRecursive("Icon").GetComponent<Image>();
    }

    public void Init(ProductionBuildingUI buildUI, ProductionRecipe recipe)
    {
        this.buildUI = buildUI;
        this.recipe = recipe;

        _icon.sprite = LoadManager.Instance.itemDataDic[recipe.outputs[0].item_id].icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (buildUI.currentBuilding.IsProducting)
        {
            return ;
        }

        if (IsSelected)
        {
            buildUI.DeSelectAll();
            buildUI.currentBuilding.selectedRecipe = null;
        }
        else
        {
            buildUI.DeSelectAll();
            Select();
            buildUI.currentBuilding.selectedRecipe = recipe;
        }
    }

    public void Select()
    {
        this.IsSelected = true;
        _backGround.color = Color.red;
    }

    public void DeSelect()
    {
        this.IsSelected = false;
        _backGround.color = Color.white;
    }
}
    