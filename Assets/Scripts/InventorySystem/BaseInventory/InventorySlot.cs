using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public InventoryContainer container;

    public InventoryItem bindItem;

    public Action<InventorySlot, InventoryItem> onItemAdded; //物品添加回调
    public Action<InventorySlot, InventoryItem> onItemRemoved; //物品移除回调

    public void SetContainer(InventoryContainer inventoryContainer)
    {
        container = inventoryContainer;
    }

    public void SetItemAmount(int v)
    {
        bindItem.SetItemAmount(v);
    }

    public ItemData AddItem(ItemData itemData, int count)
    {
        GameObject item = GameObject.Instantiate(InventorySystem.Instance.itemPrefab, this.transform);
        InventoryItem inventoryItem = item.GetComponent<InventoryItem>();

        inventoryItem.bindSlot = this;
        bindItem = inventoryItem;

        inventoryItem.SetItem(itemData, count);

        return itemData;
    }

    public void BindItem(InventoryItem inventoryItem)
    {
        if (inventoryItem == null) return;

        // 先断开原始槽位的绑定引用，防止原槽位残留导致重复操作
        var origin = inventoryItem.bindSlot;
        if (origin != null && origin.bindItem == inventoryItem)
        {
            origin.bindItem = null;
        }

        inventoryItem.bindSlot = this;
        this.bindItem = inventoryItem;

        inventoryItem.transform.SetParent(this.transform, false);
        (inventoryItem.transform as RectTransform).anchoredPosition = Vector2.zero;

        onItemAdded?.Invoke(this, inventoryItem);
    }


    public void PickUpItem(InventoryItem inventoryItem)
    {
        inventoryItem.bindSlot.onItemRemoved?.Invoke(inventoryItem.bindSlot, inventoryItem);
        inventoryItem.bindSlot.bindItem = null;

        inventoryItem.bindSlot = this;
        this.bindItem = inventoryItem;

        //UI移动
        inventoryItem.transform.SetParent(this.transform, false);
        (inventoryItem.transform as RectTransform).anchoredPosition = Vector2.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InventorySystem.Instance.focusSlot = this;

        if (bindItem != null)
        {
            StartCoroutine(ScaleOverTime(1.2f, 0.2f));

            InventorySystem.Instance.tip.SetUI(bindItem.itemData);
            InventorySystem.Instance.tip.IsOpen = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventorySystem.Instance.focusSlot = null;

        if (bindItem != null)
        {
            StartCoroutine(ScaleOverTime(1f, 0.2f));

            InventorySystem.Instance.tip.SetUI(null);
            InventorySystem.Instance.tip.IsOpen = false;
        }
    }

    private IEnumerator ScaleOverTime(float magnification, float time)
    {
        float scaleTimer = 0f;
        var _rt = bindItem._icon.GetComponent<RectTransform>();
        Vector3 originScale = _rt.localScale;
        Vector3 targetScale = Vector3.one * magnification;

        while (scaleTimer <= time)
        {
            _rt.localScale = Vector3.Lerp(originScale, targetScale, scaleTimer);
            scaleTimer += Time.deltaTime;
        }

        _rt.localScale = targetScale;
        yield return null;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        InventorySlot palmsolt = InventorySystem.Instance.palm.slotList[0];
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LeftMouseClick(palmsolt);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            RightMouseClick(palmsolt);
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            MiddleMouseClick(palmsolt);
        }

    }


    private void LeftMouseClick(InventorySlot palmsolt)
    {
        if (InventorySystem.Instance.IsPicked())
        {
            //手掌和当前slot都不为空，移动/合并
            if (bindItem != null)
            {
                //物品相同，合并
                if (bindItem.itemData.id == palmsolt.bindItem.itemData.id && bindItem.itemData.max_stack > 1)
                {
                    MergeItemforClick(palmsolt);
                }
                //物品不同，交换
                else
                {
                    ExchangeItemforClick(palmsolt);
                }
            }
            //手掌不为空，当前slot为空，放置
            else
            {
                palmsolt.bindItem.DropDownItem(this);
            }
        }
        else
        {
            //手掌为空，当前slot不为空，拾取
            if (bindItem != null)
            {
                //InventorySystem.Instance.originSlot = this;
                palmsolt.PickUpItem(this.bindItem);
            }
            //手掌为空，当前slot也为空，无操作
            else
            {
                return;
            }
        }
    }

    private void MergeItemforClick(InventorySlot palmsolt)
    {
        int totalCount = palmsolt.bindItem.itemData.current_stack + this.bindItem.itemData.current_stack;
        int overFlow = totalCount - this.bindItem.itemData.max_stack;
        if (overFlow > 0)
        {
            this.bindItem.SetItemAmount(this.bindItem.itemData.max_stack);
            palmsolt.bindItem.SetItemAmount(overFlow);
        }
        else
        {
            this.bindItem.SetItemAmount(totalCount);
            palmsolt.bindItem.SetItemAmount(0);
        }
    }

    private void ExchangeItemforClick(InventorySlot palmsolt)
    {
        //ItemData itemData = palmsolt.bindItem.itemData;
        //palmsolt.bindItem.SetItemAmount(0);

        //palmsolt.PickUpItem(this.bindItem);
        //this.AddItem(itemData, itemData.current_stack);

        var palmItem = palmsolt.bindItem;
        var slotItem = this.bindItem;

        palmsolt.BindItem(slotItem);   // 手掌拿起当前槽位物品
        this.BindItem(palmItem);       // 当前槽位放下手掌物品
    }


    private void RightMouseClick(InventorySlot palmsolt)
    {
        if (InventorySystem.Instance.IsPicked())
        {
            //手掌不为空，放下一个
            DropDownOne(palmsolt);
        }
        else
        {
            //手掌为空，slot不为空，取一个
            if (bindItem != null)
            {
                SplitStack(palmsolt);
            }
            //手掌为空，右键但当前slot也为空，无操作
            else
            {
                return ;
            }
        }
    }


    private void DropDownOne(InventorySlot palmsolt)
    {
        //1.当前slot为空
        if (bindItem == null)
        {
            if (palmsolt == null || palmsolt.bindItem == null) return;

            this.AddItem(new ItemData(palmsolt.bindItem.itemData), 1);
            palmsolt.bindItem.SetItemAmount(palmsolt.bindItem.itemData.current_stack - 1);
        }
        //2.当前slot不为空
        else
        {
            //2.1  手掌和当前槽位为同一种类
            if (bindItem.itemData.id == palmsolt.bindItem.itemData.id)
            {
                int availableStack = bindItem.itemData.max_stack - bindItem.itemData.current_stack;
                //放得下一个
                if (availableStack > 0)
                {
                    bindItem.SetItemAmount(bindItem.itemData.current_stack + 1);
                    palmsolt.bindItem.SetItemAmount(palmsolt.bindItem.itemData.current_stack - 1);
                }
                //放不下
                else
                {
                    //放不下就什么都不做
                    return;
                }
            }
            //2.2  手掌和当前槽位不为同一种类
            else
            {
                //什么都不做
                return;
            }
        }
    }


    private void SplitStack(InventorySlot palmsolt)
    {
        int pickupCount = Mathf.CeilToInt(bindItem.itemData.current_stack / 2f);
        int surplusCount = bindItem.itemData.current_stack - pickupCount;
        //print($"拆分当时{bindItem.itemData.current_stack}，本次拾取数量为{pickupCount.ToString()}");

        palmsolt.AddItem(new ItemData(bindItem.itemData), pickupCount);
        bindItem.SetItemAmount(surplusCount);
    }


    private void MiddleMouseClick(InventorySlot palmsolt)
    {

    }

    public void DeleteItem()
    {
        if (bindItem == null)
        {
            if (transform.childCount != 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i));
                }
            }
        }
        else
        {
            Destroy(bindItem.gameObject);
            bindItem = null;
        }
    }

    public void AddItemByID(int id, int count)
    {
        ItemData itemData = LoadManager.Instance.itemDataDic[id];
        AddItem(itemData, count);
    }
}
