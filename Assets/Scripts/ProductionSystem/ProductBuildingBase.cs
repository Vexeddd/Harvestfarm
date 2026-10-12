using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductBuildingBase : MonoBehaviour, IInteractalbe
{
    public ProductionRecipe selectedRecipe;

    public List<ItemRequirement> inputs = new List<ItemRequirement>();
    public List<ItemProduction> outputs = new List<ItemProduction>();
    public List<ItemFuel> fuels = new List<ItemFuel>();

    public bool IsProducting = false;
    public bool IsPaused = false;
    public bool isLoopProduce = false;

    public Action onProgressStop;
    public Action onInputConsume;
    public Action onFuelConsume;
    public Action<float, float> onProgressChanged;
    public Action<float> onFuelChanged;
    public Action onProduceFinish;

    #region IInteractable接口实现
    public virtual bool CanInteract()
    {
        return true;
    }

    public virtual string GetInteractTip()
    {
        return "按F 交互";
    }

    public virtual void Ineract(GameObject obj)
    {

    }
    #endregion

    #region 初始化
    public void InitInputs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            inputs.Add(new ItemRequirement());
        }
    }

    public void InitOutputs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            outputs.Add(new ItemProduction());
        }
    }

    public void InitFuels(int count)
    {
        for (int i = 0; i < count; i++)
        {
            fuels.Add(new ItemFuel());
        }
    }
    #endregion



    #region 生产相关
    public bool TryStartProduce()
    {
        bool res = CanStartNewProduction();
        if (res)
        {
            StartProduce();
        }
        else
        {

        }
        return res;
    }

    private bool CanStartNewProduction()
    {
        //1.没有选择配方
        if (selectedRecipe == null || selectedRecipe.id == 0)
        {
            return false;
        }

        //2.正在生成
        //废弃，因为正在生成  烹饪按钮不走这个逻辑

        //3.输入区的东西和配方不匹配, 或者 当前输入区的内容不够选中配方生产一次的

        //数量不够
        if (!HasEnoughInput())
        {
            return false;
        }

        return true;
    }

    private bool HasEnoughInput()
    {
        foreach (ItemRequirement requirement in selectedRecipe.inputs)
        {
            int total = 0;
            foreach (var input in inputs)
            {
                if (input.item_id == requirement.item_id)
                {
                    total += input.count;
                }
            }

            if (total < requirement.count)
            {
                Debug.Log("无法生产 材料不足" + requirement.item_id);
                return false;
            }
        }

        Debug.Log("有足够输入, 可以生产");
        return true;
    }

    private void StartProduce()
    {
        ConsumeInputs();
        IsProducting = true;
    }

    private void ConsumeInputs()
    {
        foreach (ItemRequirement requirement in selectedRecipe.inputs)
        {
            int need = requirement.count;
            foreach (var input in inputs)
            {
                if (need < 0)
                {
                    break;
                }

                if (input.IsEmpty || input.item_id != requirement.item_id)
                {
                    continue;
                }

                var have = input.count;
                input.count -= Mathf.Min(need, input.count);
                need -= Mathf.Min(need, have);

                if (input.count <= 0)
                {
                    input.item_id = 0;
                    input.count = 0;
                }
            }
        }

        onInputConsume?.Invoke();
    }


    public void FinishProduce()
    {
        //输出槽添加物品

        //遍历输出结果
        for (int i = 0; i < selectedRecipe.outputs.Count; i++)
        {
            var id = selectedRecipe.outputs[i].item_id;
            var count = selectedRecipe.outputs[i].count;
            for (int j = 0; j < outputs.Count; j++)
            {
                //空槽
                if (outputs[j].IsEmpty)   //虽然是输出数据，但是在UI中是和槽位一一对应的
                {
                    outputs[j].item_id = id;
                    outputs[j].count = count;
                }
                else
                {
                    //已有
                    if (outputs[j].item_id == id)
                    {
                        outputs[j].count += count;
                    }
                }
            }
        }

        onProduceFinish?.Invoke();


        if (isLoopProduce)
        {
            if (CanStartNewProduction())
            {
                StartProduce();
            }
            else
            {
                StopCirculProduction();
            }
        }
        else
        {
            StopCirculProduction();
        }
    }


    public void StopCirculProduction()
    {
        processTimer = 0f;
        intervalTimer = 0f;

        IsProducting = false;
        onProgressStop?.Invoke();
    }
    #endregion


    private float processTimer;
    private float intervalTimer;
    public float fireTimer = 0;
    public virtual void Update()
    {
        if (IsPaused)
        {
            return;
        }

        if (!IsProducting)
        {
            return;
        }

        //判断燃料（火力值是否足够）
        if (HasFire())
        {
            UpdateFire();
        }
        else
        {
            ItemFuel fuel = HasAnyFuel();
            if (fuel == null)
            {
                return;
            }
            else
            {
                //消耗燃料
                ConsumeFuel(fuel);
                UpdateFire();
            }

        }

        //开始生产
        processTimer += Time.deltaTime;
        intervalTimer += Time.deltaTime;
        if (intervalTimer >= 0.1f)
        {
            intervalTimer = 0f;
            onProgressChanged?.Invoke(processTimer, selectedRecipe.duration);
        }

        if (processTimer >= selectedRecipe.duration)
        {
            processTimer = 0;
            FinishProduce();
        }
    }


    private bool HasFire()
    {
        return fireTimer > 0f;
    }

    private float fireInterval = 0f;
    private void UpdateFire()
    {
        fireTimer -= Time.deltaTime;
        fireInterval += Time.deltaTime;

        if (fireInterval >= 0.1f)
        {
            fireInterval = 0f;
            onFuelChanged?.Invoke(fireTimer);
        }

        if (fireTimer <= 0)
        {
            fireTimer = 0f;
        }
    }

    private ItemFuel HasAnyFuel()
    {
        foreach (var fuel in fuels)
        {
            if (fuel.IsEmpty)
            {
                continue;
            }

            if (fuel.item_id == 102)
            {
                return fuel;
            }
        }

        return null;
    }

    private void ConsumeFuel(ItemFuel fuel)
    {
        fireTimer = 20f;
        fuel.count -= 1;

        if (fuel.count == 0)
        {
            fuel.item_id = 0;
        }

        onFuelConsume?.Invoke();
    }
}
