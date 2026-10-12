using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryContainer : UIContainer
{
    private Transform _layout;
    public List<InventorySlot> slotList = new List<InventorySlot>();
    // Start is called before the first frame update
    void Start()
    {
            
    }



    public virtual List<InventorySlot> InitContainer(int slotCount)
    {
        _layout = transform.Find("SlotLayout").transform; 

        //Éú³É²å²Û
        for (int i = 0; i < slotCount; i++)
        {
            GameObject go = GameObject.Instantiate(InventorySystem.Instance.slotPrefab, _layout);

            //slot½Å±¾
            InventorySlot slot = go.AddComponent<InventorySlot>();
            slotList.Add(slot);
            slot.SetContainer(this);
        }

        return slotList;
    }

    public InventorySlot FindAvailSlot(int id)
    {
        foreach (var slot in slotList)
        {
            if (slot.bindItem != null && slot.bindItem.itemData.id == id && slot.bindItem.itemData.current_stack < slot.bindItem.itemData.max_stack)
            {
                return slot;
            }
        }

        return null;
    }

    public InventorySlot FindEmptySlot()
    {
        foreach (var slot in slotList)
        {
            if (slot.bindItem == null)
            {
                return slot;
            }
        }

        return null;
    }

    public List<ItemData> GetItemList()
    {
        var list = new List<ItemData>();
        foreach (var slot in slotList)
        {
            if (slot.bindItem != null)
            {
                list.Add(new ItemData(slot.bindItem.itemData));
            }
            else
            {
                list.Add(null);
            }
        }

        return list;
    }

    public void ClearItems()
    {
        foreach (var slot in slotList)
        {
            if (slot.bindItem != null)
            {
                Destroy(slot.bindItem.gameObject);
                slot.bindItem = null;
            }
        }
    }
}
