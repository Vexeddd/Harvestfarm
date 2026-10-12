using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChickenCoopUI : InventoryContainer
{
    private static ChickenCoopUI _instance;
    public static ChickenCoopUI Instance
    {
        get { return _instance; }
    }
    protected virtual void Awake()
    {
        _instance = this as ChickenCoopUI;

        closeBtn.onClick.AddListener(() => {
            Close();
        });
    }



    public ChickenCoop currentCoop;
    public List<InventorySlot> chickenSlots;

    private Dictionary<InventorySlot, Chicken> slotChickenMap = new Dictionary<InventorySlot, Chicken>();

    public Button closeBtn;


    // Start is called before the first frame update
    void Start()
    {
        chickenSlots = InitContainer(4);

        for (int i = 0; i < chickenSlots.Count; i++)
        {
            chickenSlots[i].onItemAdded += OnChickenAdded;
            chickenSlots[i].onItemRemoved += OnChickenRemoved;
        }
    }

    private void OnChickenAdded(InventorySlot slot, InventoryItem item)
    {
        // 该槽位已有一只鸡时先移除旧的，避免重复生成
        if (slotChickenMap.TryGetValue(slot, out Chicken oldChicken))
        {
            currentCoop.RemoveChicken(oldChicken);
        }

        Chicken chicken = currentCoop.AddChicken(item);
        slotChickenMap[slot] = chicken;
    }

    private void OnChickenRemoved(InventorySlot slot, InventoryItem item)
    {
        if (slotChickenMap.TryGetValue(slot, out Chicken chicken))
        {
            currentCoop.RemoveChicken(chicken);
            slotChickenMap.Remove(slot);
        }
    }


    public void Close()
    {
        currentCoop.coopBrand.isInteracting = false;
        currentCoop = null;

        this.IsOpen = false;
    }

    public void Open(ChickenCoop chickenCoop)
    {
        this.currentCoop = chickenCoop;

        this.IsOpen = true;
    }
}
