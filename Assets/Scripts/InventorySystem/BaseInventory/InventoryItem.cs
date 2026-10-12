using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public ItemData itemData;
    public InventorySlot bindSlot;

    public Image _icon;
    public TMP_Text _text;

    private void Awake()
    {
        _icon = transform.Find("Icon").GetComponent<Image>();
        _text = transform.Find("Text").GetComponent<TMP_Text>();
    }

    internal void SetItemAmount(int count)
    {
        //如果数量为0 , 直接销毁
        if (count == 0)
        {
            itemData.current_stack = 0;
            bindSlot.bindItem = null;

            Destroy(gameObject);
            return;
        }

        itemData.current_stack = count;

        _text.text = count.ToString();
    }

    internal void SetItem(ItemData itemData, int count)
    {
        this.itemData = itemData;

        SetItemAmount(count);
        _icon.sprite = itemData.icon;
    }

    public void OnDrag(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        InventorySlot palmsolt = InventorySystem.Instance.palm.slotList[0];
        if (palmsolt.bindItem != null)
        {
            eventData.pointerDrag = null;
            return;
        }

        InventorySystem.Instance.originSlot = bindSlot;
        _icon.raycastTarget = false;

        palmsolt.PickUpItem(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _icon.raycastTarget = true;

        InventorySlot originSlot = InventorySystem.Instance.originSlot;
        InventorySlot focusSlot = InventorySystem.Instance.focusSlot;
        InventorySlot palmSlot = InventorySystem.Instance.palm.slotList[0];
        if (focusSlot != null)
        {
            //1.放入空格
            if (focusSlot.bindItem == null)
            {
                DropDownItem(focusSlot);
            }
            //2.拖拽目标slot内有物体
            else
            {
                //2.1 同物体合并
                if (focusSlot.bindItem.itemData.id == palmSlot.bindItem.itemData.id)
                {
                    MergeItemforDrag(palmSlot);
                }
                //2.2 不同物体交换
                else if (focusSlot.bindItem.itemData.id != palmSlot.bindItem.itemData.id)
                {
                    ExchangeItemforDrag(palmSlot);
                }
            }
        }
        else
        {
            if (originSlot == null) return;

            var focusContainer = InventorySystem.Instance.focusContainer;

            if (focusContainer == null)
            {
                Debug.Log($"丢下了：{itemData.current_stack}个{itemData.name}");
                DiscardItem(false);
            }
            else
            {
                if (focusContainer is Trashcan)
                {
                    Debug.Log($"销毁了：{itemData.current_stack}个{itemData.name}");
                    DiscardItem(true);
                }
                else
                {
                    originSlot.PickUpItem(this);
                    DropDownItem(originSlot);
                }
            }

        }
    }

    private void DiscardItem(bool isDestroy)
    {
        if (!isDestroy)
        {
            InventorySystem.Instance.CreatItemInWorld(this.itemData);
        }
        SetItemAmount(0);
        InventorySystem.Instance.palm.slotList[0].bindItem = null;
        InventorySystem.Instance.originSlot = null; 
    }

    private void MergeItemforDrag(InventorySlot palmSlot)
    {
        var originSlot = InventorySystem.Instance.originSlot;
        var foucsSlot = InventorySystem.Instance.focusSlot;

        int totalCount = palmSlot.bindItem.itemData.current_stack + foucsSlot.bindItem.itemData.current_stack;
        int overFlow = palmSlot.bindItem.itemData.current_stack + foucsSlot.bindItem.itemData.current_stack - foucsSlot.bindItem.itemData.max_stack;

        //溢出了，合并，剩余的取消
        if (overFlow > 0)
        {
            foucsSlot.SetItemAmount(foucsSlot.bindItem.itemData.max_stack);

            palmSlot.bindItem.SetItemAmount(overFlow);
            originSlot.BindItem(palmSlot.bindItem);

            palmSlot.bindItem = null;
        }
        //没有溢出，合并   
        else
        {
            foucsSlot.SetItemAmount(totalCount);
            palmSlot.SetItemAmount(0);
        }
    }

    private void ExchangeItemforDrag(InventorySlot palmSlot)
    {
        var originSlot = InventorySystem.Instance.originSlot;
        var foucsSlot = InventorySystem.Instance.focusSlot;

        originSlot.BindItem(foucsSlot.bindItem);
        foucsSlot.BindItem(palmSlot.bindItem);

        palmSlot.bindItem = null;
    }

    public void DropDownItem(InventorySlot focusSlot)
    {
        focusSlot.BindItem(this);

        InventorySystem.Instance.palm.slotList[0].bindItem = null;
    }
}



