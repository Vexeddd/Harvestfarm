using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProductionBuildingUI : UIContainer
{
    public List<InventorySlot> inputs; //输入区插槽
    public List<InventorySlot> outputs; //输出区插槽
    public List<InventorySlot> fuels; //燃料插槽



    public GameObject recipeItem_prefab; //配方预制体
    public Transform _recipesContent; //配方内容区
    public List<RecipeItemUI> recipeUIList = new List<RecipeItemUI>();

    public ProductBuildingBase currentBuilding;

    public Button closeBtn;
    public Button executeBtn;
    public Image progressSlider;
    public Toggle isLoop;
    public Image fuelSlider;
    public Button pauseButton;

    public virtual void Awake()
    {
        closeBtn = transform.FindRecursive("CloseButton").GetComponent<Button>();
        closeBtn.onClick.AddListener(() => {
            Close();
        });

        executeBtn = transform.FindRecursive("ExecuteButton").GetComponent<Button>();
        executeBtn.onClick.AddListener(() => 
        {
            if (currentBuilding.IsProducting)
            {
                currentBuilding.StopCirculProduction();
            }
            else
            {
                bool res = currentBuilding.TryStartProduce(); 
                executeBtn.transform.GetChild(0).gameObject.SetActive(!res);
                executeBtn.transform.GetChild(1).gameObject.SetActive(res);
            }
        });

        pauseButton = transform.FindRecursive("PauseButton").GetComponent<Button>();
        pauseButton.onClick.AddListener(() => 
        {
            if (currentBuilding.IsPaused)
            {
                currentBuilding.IsPaused = false;
                pauseButton.transform.GetChild(0).gameObject.SetActive(true);
                pauseButton.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                currentBuilding.IsPaused = true;
                pauseButton.transform.GetChild(0).gameObject.SetActive(false);
                pauseButton.transform.GetChild(1).gameObject.SetActive(true);
            }
        });

        progressSlider = transform.FindRecursive("ProgressSlider").GetComponent<Image>();
        fuelSlider = transform.FindRecursive("FuelSlider").GetComponent<Image>();
        _recipesContent = transform.FindRecursive("RecipesContent");

        isLoop = transform.FindRecursive("IsLoop").GetComponent<Toggle>();
        isLoop.onValueChanged.AddListener((isOn) =>
        {
            currentBuilding.isLoopProduce = isOn;
        });

        var input_Container = this.transform.FindRecursive("InputContainer").FindRecursive("SlotLayout");
        for (int i = 0; i < input_Container.childCount; i++)
        {
            InventorySlot slot = input_Container.GetChild(i).AddComponent<InventorySlot>();
            inputs.Add(slot);
        }

        var output_Container = this.transform.FindRecursive("OutPutContainer").FindRecursive("SlotLayout");
        for (int i = 0; i < output_Container.childCount; i++)
        {
            InventorySlot slot = output_Container.GetChild(i).AddComponent<InventorySlot>();
            outputs.Add(slot);
        }

        var fuel_Container = this.transform.FindRecursive("FuelContainer").FindRecursive("SlotLayout");
        for (int i = 0; i < fuel_Container.childCount; i++)
        {
            InventorySlot slot = fuel_Container.GetChild(i).AddComponent<InventorySlot>();
            fuels.Add(slot);
        }

        //监听移入移出事件
        foreach (var slot in inputs)
        {
            slot.onItemAdded += OnInputItemAdded;
            slot.onItemRemoved += OnInputItemRemoved;
        }

        foreach (var slot in outputs)
        {
            slot.onItemAdded += OnOutputItemAdded;
            slot.onItemRemoved += OnOutputItemRemoved;
        }

        foreach (var slot in fuels)
        {
            slot.onItemAdded += OnFuelItemAdded;
            slot.onItemRemoved += OnFuelItemRemoved;
        }
    }


    public virtual void Start()
    {

    }


    public virtual void Open(ProductBuildingBase productBuildingBase)
    {
        this.currentBuilding = productBuildingBase;
        RefreshAll();

        //监听事件
        currentBuilding.onProgressStop += OnProgressStopHandle;
        currentBuilding.onInputConsume += OnInputConsumeHandle;
        currentBuilding.onProgressChanged += OnProgressChangedHandle;
        currentBuilding.onProduceFinish += OnProduceFinishHandle;
        currentBuilding.onFuelConsume += OnFuelConsumeHandle;
        currentBuilding.onFuelChanged += OnFuelChangedHandle;

        this.IsOpen = true;
    }

    public virtual void Close()
    {
        currentBuilding.onProgressStop -= OnProgressStopHandle;
        currentBuilding.onInputConsume -= OnInputConsumeHandle;
        currentBuilding.onProgressChanged -= OnProgressChangedHandle;
        currentBuilding.onProduceFinish -= OnProduceFinishHandle;
        currentBuilding.onFuelConsume -= OnFuelConsumeHandle;
        currentBuilding.onFuelChanged -= OnFuelChangedHandle;

        currentBuilding = null;

        this.IsOpen = false;

    }


    private void OnProgressStopHandle()
    {
        executeBtn.transform.GetChild(0).gameObject.SetActive(true);
        executeBtn.transform.GetChild(1).gameObject.SetActive(false);

        progressSlider.fillAmount = 0;
    }

    private void OnInputConsumeHandle()
    {
        RefreshInputContainer();
    }

    private void OnProgressChangedHandle(float arg1, float arg2)
    {
        progressSlider.fillAmount = arg1 / arg2;
    }

    private void OnProduceFinishHandle()
    {
        RefreshOutputContainer();
    }

    private void OnFuelConsumeHandle()
    {
        RefreshFuelContainer();
    }

    private void OnFuelChangedHandle(float obj)
    {
        fuelSlider.fillAmount = obj / 20f;
    }



    public void CreateRicepes(List<ProductionRecipe> recipes)
    {
        foreach (var recipe in recipes)
        {
            GameObject go = GameObject.Instantiate(recipeItem_prefab, _recipesContent);

            RecipeItemUI rUI = go.AddComponent<RecipeItemUI>();
            rUI.Init(this, recipe);

            recipeUIList.Add(rUI);
        }
    }

    #region 刷新UI

    private void RefreshAll()
    {
        RefreshInputContainer();
        RefreshOutputContainer();
        RefreshFuelContainer();

        RefreshRecipeContainer();
    }

    private void RefreshRecipeContainer()
    {
        if (currentBuilding.selectedRecipe == null)
        {
            return;
        }
        DeSelectAll();

        foreach (var recipeUI in recipeUIList)
        {
            if (recipeUI.recipe.id == currentBuilding.selectedRecipe.id)
            {
                recipeUI.Select();

                return;
            }
        }
    }

    private void RefreshContainer(ProductionContainerType containerType)
    {
        var container = GetContainer(containerType);

        //清除数据
        for (int i = 0; i < container.Count; i++)
        {
            if (container[i].transform.childCount > 0)
            {
                container[i].DeleteItem();
            }
        }

        //载入UI
        switch (containerType)
        {
            case ProductionContainerType.InputContainer:
                var ip = currentBuilding.inputs;
                for (int i = 0; i < ip.Count; i++)
                {
                    int id = ip[i].item_id;
                    int count = ip[i].count;
                    if (id != 0 && count != 0)
                    {
                        inputs[i].AddItemByID(id, count);
                    }
                }
                break;
            case ProductionContainerType.OutputContainer:
                var op = currentBuilding.outputs;
                for (int i = 0; i < op.Count; i++)
                {
                    int id = op[i].item_id;
                    int count = op[i].count;
                    if (id != 0 && count != 0)
                    {
                        outputs[i].AddItemByID(id, count);
                    }
                }
                break;
            case ProductionContainerType.FuelContainer:
                var fu = currentBuilding.fuels;
                for (int i = 0; i < fu.Count; i++)
                {
                    int id = fu[i].item_id;
                    int count = fu[i].count;
                    if (id != 0 && count != 0)
                    {
                        fuels[i].AddItemByID(id, count);
                    }
                }
                break;
        }
    }

    private void RefreshInputContainer()
    {
        RefreshContainer(ProductionContainerType.InputContainer);
    }

    private void RefreshOutputContainer()
    {
        RefreshContainer(ProductionContainerType.OutputContainer);
    }

    private void RefreshFuelContainer()
    {
        RefreshContainer(ProductionContainerType.FuelContainer);
    }
    #endregion

    #region Slot移入移出实现
    private void OnInputItemAdded(InventorySlot slot, InventoryItem item)
    {
        OnItemAddHandle(slot, item, ProductionContainerType.InputContainer);
    }

    private void OnInputItemRemoved(InventorySlot slot, InventoryItem item)
    {
        OnItemRemoveHandle(slot, item, ProductionContainerType.InputContainer);
    }

    private void OnOutputItemAdded(InventorySlot slot, InventoryItem item)
    {
        OnItemAddHandle(slot, item, ProductionContainerType.OutputContainer);
    }


    private void OnOutputItemRemoved(InventorySlot slot, InventoryItem item)
    {
        OnItemRemoveHandle(slot, item, ProductionContainerType.OutputContainer);
    }

    private void OnFuelItemAdded(InventorySlot slot, InventoryItem item)
    {
        OnItemAddHandle(slot, item, ProductionContainerType.FuelContainer);
    }

    private void OnFuelItemRemoved(InventorySlot slot, InventoryItem item)
    {
        OnItemRemoveHandle(slot, item, ProductionContainerType.FuelContainer);
    }

    public void OnItemAddHandle(InventorySlot slot, InventoryItem item, ProductionContainerType containerType)
    {
        List<InventorySlot> container = GetContainer(containerType);

        for (int i = 0; i < container.Count; i++)
        {
            //遍历查找目标slot
            if (slot == container[i])
            {
                switch (containerType)
                {
                    case ProductionContainerType.InputContainer:
                        currentBuilding.inputs[i].item_id = item.itemData.id;
                        currentBuilding.inputs[i].count = item.itemData.current_stack;
                        break;
                    case ProductionContainerType.OutputContainer:
                        currentBuilding.outputs[i].item_id = item.itemData.id;
                        currentBuilding.outputs[i].count = item.itemData.current_stack;
                        break;
                    case ProductionContainerType.FuelContainer:
                        currentBuilding.fuels[i].item_id = item.itemData.id;
                        currentBuilding.fuels[i].count = item.itemData.current_stack;
                        break;
                }
            }
        }
    }

    public void OnItemRemoveHandle(InventorySlot slot, InventoryItem item, ProductionContainerType containerType)
    {
        List<InventorySlot> container = GetContainer(containerType);

        for (int i = 0; i < container.Count; i++)
        {
            //遍历查找目标slot
            if (slot == container[i])
            {
                switch (containerType)
                {
                    case ProductionContainerType.InputContainer:
                        currentBuilding.inputs[i].item_id = 0;
                        currentBuilding.inputs[i].count = 0;
                        break;
                    case ProductionContainerType.OutputContainer:
                        currentBuilding.outputs[i].item_id = 0;
                        currentBuilding.outputs[i].count = 0;
                        break;
                    case ProductionContainerType.FuelContainer:
                        currentBuilding.fuels[i].item_id = 0;
                        currentBuilding.fuels[i].count = 0;
                        break;
                }
            }
        }
    }
    public List<InventorySlot> GetContainer(ProductionContainerType containerType)
    {
        List<InventorySlot> container = null;

        switch (containerType)
        {
            case ProductionContainerType.InputContainer:
                container = inputs;
                break;
            case ProductionContainerType.OutputContainer:
                container = outputs;
                break;
            case ProductionContainerType.FuelContainer:
                container = fuels;
                break;
        }

        return container;
    }

    public void DeSelectAll()
    {
        foreach (var recipeUI in recipeUIList)
        {
            recipeUI.DeSelect();
        }
    }
    #endregion




}


public enum ProductionContainerType
{
    InputContainer, //输入区
    OutputContainer, //输出区
    FuelContainer, //燃料区
}
