using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBar : InventoryContainer
{
    private int selectedIndex = 0;
    private List<KeyCode> keyCodes = new List<KeyCode>();

    public GameObject selectedbox;

    private void Awake()
    {
        if(selectedbox != null)
            { selectedbox.SetActive(false); }

        keyCodes.Add(KeyCode.Alpha1);
        keyCodes.Add(KeyCode.Alpha2);
        keyCodes.Add(KeyCode.Alpha3);
        keyCodes.Add(KeyCode.Alpha4);
        keyCodes.Add(KeyCode.Alpha5);
        keyCodes.Add(KeyCode.Alpha6);
        keyCodes.Add(KeyCode.Alpha7);
        keyCodes.Add(KeyCode.Alpha8);
        keyCodes.Add(KeyCode.Alpha9);
        keyCodes.Add(KeyCode.Alpha0);
    }


    private void Update()
    {
        if (Input.anyKeyDown)
        {
            foreach (var key in keyCodes)
            {
                if (Input.GetKeyDown(key))
                {
                    SwitchSlot(keyCodes.IndexOf(key));
                }
            }
        }

        var item = slotList[selectedIndex].bindItem;
        if (item != null && item.itemData.item_type == ItemType.Food && Input.GetKeyDown(KeyCode.E))
        {
            //物品减少
            item.SetItemAmount(item.itemData.current_stack - 1);
            //触发buff
            EventBus.Publish(new EatFoodEvent(item.itemData));
        }
    }

    private void SwitchSlot(int index)
    {
        selectedIndex = index;

        selectedbox.SetActive(true);
        selectedbox.transform.position = slotList[selectedIndex].transform.position;

        var item = slotList[selectedIndex].bindItem;
        if (item == null)
        {
            SwitchItem(null);
        }
        else
        {

            SwitchItem(item);
        }
    }

    private void SwitchItem(InventoryItem item)
    {
        EventBus.Publish ( new ToolbarItemChangedEvent(item));
    }

    //internal void ReduceItemAmount(int amount)
    //{
    //    slotList[selectedIndex].bindItem.SetItemAmount(slotList[selectedIndex].bindItem.itemData.current_stack - 1);
    //}
    public void ReduceItemAmount(int amount)
    {
        var slot = slotList[selectedIndex];
        if (slot.bindItem == null) return;
        slot.bindItem.SetItemAmount(slot.bindItem.itemData.current_stack - amount);
    }
}
