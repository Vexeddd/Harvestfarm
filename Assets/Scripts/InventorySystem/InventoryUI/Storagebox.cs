using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storagebox : InventoryContainer
{
    public void OpenBox(List<ItemData> itemDatas)
    {
        this.IsOpen = true;
        InventorySystem.Instance.backpack.IsOpen = true;

        for (int i = 0; i < itemDatas.Count; i++)
        {
            if (itemDatas[i] != null)
            {
                slotList[i].AddItem(itemDatas[i], itemDatas[i].current_stack);
            }
        }
    }

    public List<ItemData> CloseBox()
    {
        InventorySystem.Instance.backpack.IsOpen = false;
        this.IsOpen = false;

        List<ItemData> storageItems = new List<ItemData>();
        storageItems = GetItemList();
        ClearItems();

        return storageItems;
    }

}
