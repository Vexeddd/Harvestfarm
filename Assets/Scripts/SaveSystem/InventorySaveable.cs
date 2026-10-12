using System.Collections.Generic;

public class InventorySaveable : ISaveable
{
    public void OnSave(SaveData data)
    {
        data.containers = new List<ContainerSave>();

        SaveContainer(data, "toolBar", InventorySystem.Instance.toolBar);
        SaveContainer(data, "backpack", InventorySystem.Instance.backpack);
        SaveContainer(data, "storagebox", InventorySystem.Instance.storagebox);
    }

    private void SaveContainer(SaveData data, string name, InventoryContainer container)
    {
        var cs = new ContainerSave { name = name };

        foreach (var slot in container.slotList)
        {
            if (slot.bindItem == null)
            {
                cs.slots.Add(null);
                continue;
            }

            var itd = slot.bindItem.itemData;
            var stack = new ItemStackSave
            {
                id = itd.id,
                count = itd.current_stack,
                uuid = itd.uuid,
            };

            // Durability / ammo / water live in StateManager, keyed by uuid.
            if (!string.IsNullOrEmpty(itd.uuid))
            {
                var toolState = StateManager.GetState<ToolRuntimeState>(itd.uuid);
                if (toolState != null)
                {
                    stack.state = new RuntimeStateSave
                    {
                        durability = toolState.current_duration,
                        water = toolState.current_water,
                    };
                }
                else
                {
                    var weaponState = StateManager.GetState<WeaponRuntimeState>(itd.uuid);
                    if (weaponState != null)
                    {
                        stack.state = new RuntimeStateSave
                        {
                            durability = weaponState.current_duability,
                            ammo = weaponState.current_ammo,
                        };
                    }
                }
            }

            cs.slots.Add(stack);
        }

        data.containers.Add(cs);
    }

    public void OnLoad(SaveData data)
    {
        if (data.containers == null) return;

        foreach (var cs in data.containers)
        {
            var container = GetContainer(cs.name);
            if (container == null) continue;

            container.ClearItems();

            for (int i = 0; i < cs.slots.Count && i < container.slotList.Count; i++)
            {
                var stack = cs.slots[i];
                if (stack == null) continue;

                ItemData template;
                if (!LoadManager.Instance.itemDataDic.TryGetValue(stack.id, out template)) continue;

                // Always restore from a copy so the shared template in LoadManager stays untouched.
                var copy = new ItemData(template);
                copy.current_stack = stack.count;
                copy.uuid = stack.uuid;

                // Re-register runtime state so durability / ammo / water survive the load.
                if (stack.state != null && !string.IsNullOrEmpty(copy.uuid))
                {
                    if (copy.item_type == ItemType.Tool)
                    {
                        var ts = new ToolRuntimeState(copy.uuid);
                        ts.current_duration = (int)stack.state.durability;
                        ts.current_water = stack.state.water;
                        StateManager.RegisterState(copy.uuid, ts);
                    }
                    else if (copy.item_type == ItemType.Weapon)
                    {
                        var ws = new WeaponRuntimeState(copy.uuid);
                        ws.current_duability = stack.state.durability;
                        ws.current_ammo = stack.state.ammo;
                        StateManager.RegisterState(copy.uuid, ws);
                    }
                }

                container.slotList[i].AddItem(copy, stack.count);
            }
        }
    }

    private InventoryContainer GetContainer(string name)
    {
        switch (name)
        {
            case "toolBar": return InventorySystem.Instance.toolBar;
            case "backpack": return InventorySystem.Instance.backpack;
            case "storagebox": return InventorySystem.Instance.storagebox;
            default: return null;
        }
    }
}
