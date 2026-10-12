using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : SingletonAutoMono<InventorySystem>,
    ICommandHandler<CommandReduceItemAmount>,
    ICommandHandler<CommandReduceInventoryAmount>
{
    public GameObject slotPrefab;
    public GameObject itemPrefab;
    public GameObject shopitemPrefab;

    public ToolBar toolBar;
    public Backpack backpack;
    public Storagebox storagebox;
    public Shop shop;
    public Palm palm;
    public Tip tip;

    public InventorySlot focusSlot;
    public InventorySlot originSlot;
    public UIContainer focusContainer;

    private Vector3 dropDownOffset = Vector3.up * 0.5f;
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();

        slotPrefab = Resources.Load<GameObject>("Prefab/Slot");
        itemPrefab = Resources.Load<GameObject>("Prefab/Item");
        shopitemPrefab = Resources.Load<GameObject>("Prefab/ShopItem");

        toolBar = GameObject.Find("ToolBar").GetComponent<ToolBar>();
        backpack = GameObject.Find("Backpack").GetComponent<Backpack>();
        storagebox = GameObject.Find("Storagebox").GetComponent<Storagebox>();
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        palm = GameObject.Find("Palm").GetComponent<Palm>();
        tip = GameObject.Find("Tip").GetComponent<Tip>();

        toolBar.InitContainer(10);
        backpack.InitContainer(32);
        storagebox.InitContainer(8);
        palm.InitContainer(1);

        CommandBus.Register<CommandReduceItemAmount>(this);
        CommandBus.Register<CommandReduceInventoryAmount>(this);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //尝试添加物品ByID
    public bool TryAddItem(int id, int count = 1)
    {
        ItemData item = LoadManager.Instance.itemDataDic[id];

        bool res = TryAddItem(item, count);
        return res;
    }


    //尝试添加物品
    public bool TryAddItem(ItemData itemData, int count)
    {
        int surplus = count;

        if (itemData.has_state)
        {
            while (surplus > 0)
            {
                var itd = TryAddItemForState(itemData, 1);

                if (string.IsNullOrEmpty(itd.uuid))
                {
                    var uuid = StateManager.GenerateUUID();
                    itd.uuid = uuid;
                }

                surplus--;
            }

            return true;
        }

        //1.添加到已有工具栏
        while (surplus > 0)
        {
            InventorySlot availSlot = FindAvailSlot(toolBar, itemData.id);
            if (availSlot == null) break;

            int overflow = surplus - (availSlot.bindItem.itemData.max_stack - availSlot.bindItem.itemData.current_stack);
            //1.溢出
            if (overflow > 0)
            {
                availSlot.SetItemAmount(availSlot.bindItem.itemData.max_stack);
                surplus = overflow;
            }
            else
            {
                availSlot.SetItemAmount(surplus + availSlot.bindItem.itemData.current_stack);

                surplus = 0;
                return true;
            }
        }

        //2.添加到已有背包
        while (surplus > 0)
        {
            InventorySlot availSlot = FindAvailSlot(backpack, itemData.id);
            if (availSlot == null) break;

            int overflow = surplus - (availSlot.bindItem.itemData.max_stack - availSlot.bindItem.itemData.current_stack);
            //1.溢出
            if (overflow > 0)
            {
                availSlot.SetItemAmount(availSlot.bindItem.itemData.max_stack);
                surplus = overflow;
            }
            else
            {
                availSlot.SetItemAmount(surplus + availSlot.bindItem.itemData.current_stack);

                surplus = 0;
                return true;
            }
        }

        //3.添加到工具栏空位
        while (surplus > 0)
        {
            InventorySlot emptySlot = FindEmptySlot(toolBar);
            if (emptySlot == null) break;

            int overflow = surplus - itemData.max_stack;
            if (overflow > 0)
            {
                emptySlot.AddItem(new ItemData(itemData), itemData.max_stack);
                surplus = overflow;
            }
            else
            {
                emptySlot.AddItem(new ItemData(itemData), surplus);

                surplus = 0;
                return true;
            }
        }

        //4.添加到背包空位
        while (surplus > 0)
        {
            InventorySlot emptySlot = FindEmptySlot(backpack);
            if (emptySlot == null) break;

            int overflow = surplus - itemData.max_stack;
            if (overflow > 0)
            {
                emptySlot.AddItem(new ItemData(itemData), itemData.max_stack);
                surplus = overflow;
            }
            else
            {
                emptySlot.AddItem(new ItemData(itemData), surplus);

                surplus = 0;
                return true;
            }
        }

        //5. Todo : 完全溢出丢到大世界
        itemData.current_stack = surplus;
        CreatItemInWorld(new ItemData(itemData));

        return false;
    }

    private ItemData TryAddItemForState(ItemData itemData, int count = 1)
    {
        var surplus = count;

        ItemData itd;
        //3.添加到工具栏空位
        while (surplus > 0)
        {
            InventorySlot emptySlot = FindEmptySlot(toolBar);
            if (emptySlot == null) break;

            int overflow = surplus - itemData.max_stack;
            //if (overflow > 0)
            //{
            //    emptySlot.AddItem(new ItemData(itemData), itemData.max_stack);
            //    surplus = overflow;
            //}
            //else
            //{
            itd = emptySlot.AddItem(new ItemData(itemData), surplus);

            surplus = 0;
            return itd;
            //}
        }

        //4.添加到背包空位
        while (surplus > 0)
        {
            InventorySlot emptySlot = FindEmptySlot(backpack);
            if (emptySlot == null) break;

            int overflow = surplus - itemData.max_stack;
            //if (overflow > 0)
            //{
            //    emptySlot.AddItem(new ItemData(itemData), itemData.max_stack);
            //    surplus = overflow;
            //}
            //else
            //{
            itd = emptySlot.AddItem(new ItemData(itemData), surplus);

            surplus = 0;
            return itd;
            //}
        }
        //5. Todo : 完全溢出丢到大世界
        itemData.current_stack = surplus;
        itd = new ItemData(itemData);
        CreatItemInWorld(itd);

        return itd;
    }

    private InventorySlot FindAvailSlot(InventoryContainer container, int id)
    {
        return container.FindAvailSlot(id);
    }

    private InventorySlot FindEmptySlot(InventoryContainer container)
    {
        return container.FindEmptySlot();
    }

    public bool IsPicked()
    {
        return palm.slotList[0].bindItem != null;
    }

    public void CreatItemInWorld(ItemData itemData)
    {
        var collectItem_prefab = LoadManager.Instance.GetMappingPrefab(itemData.id);
        if (collectItem_prefab != null)
        {
            GameObject go = GameObject.Instantiate(collectItem_prefab, Player.Instance.transform.position + dropDownOffset, Quaternion.identity);

            CollectItem collectItem = go.GetComponent<CollectItem>();
            if (collectItem == null)
            {
                collectItem = go.AddComponent<CollectItem>();
            }
            collectItem.SetData(new ItemData(itemData));
        }
        else
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Prop/DropItem"));
            go.transform.position = Player.Instance.transform.position + dropDownOffset;

            DropItem dropItem = go.GetComponent<DropItem>();
            if (dropItem == null)
            {
                dropItem = go.AddComponent<DropItem>();
            }
            dropItem.SetData(new ItemData(itemData));
        }
    }

    public void CreatItemInWorld(ItemData itemData, Vector3 pos)
    {
        var collectItem_prefab = LoadManager.Instance.GetMappingPrefab(itemData.id);
        if (collectItem_prefab != null)
        {
            GameObject go = GameObject.Instantiate(collectItem_prefab, pos + dropDownOffset, Quaternion.identity);

            CollectItem collectItem = go.GetComponent<CollectItem>();
            if (collectItem == null)
            {
                collectItem = go.AddComponent<CollectItem>();
            }
            collectItem.SetData(new ItemData(itemData));
        }
        else
        {
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/Prop/DropItem"));
            go.transform.position = pos + dropDownOffset;

            DropItem dropItem = go.GetComponent<DropItem>();
            if (dropItem == null)
            {
                dropItem = go.AddComponent<DropItem>();
            }
            dropItem.SetData(new ItemData(itemData));
        }
    }



    public bool isUIOpen()
    {
        return backpack.IsOpen || storagebox.IsOpen || shop.IsOpen;
    }


    public CommandResult Handle(CommandReduceItemAmount command)
    {
        toolBar.ReduceItemAmount(command.amount);
        return CommandResult.OK();
    }

    public CommandResult Handle(CommandReduceInventoryAmount command)
    {
        int consume = command.amount;

        //优先检查工具栏
        if (consume != 0)
        {
            foreach (var slot in toolBar.slotList)
            {
                if (consume == 0)
                {
                    break;
                }

                if (slot.bindItem != null && slot.bindItem.itemData.id == command.item_id)
                {
                    int dec = Mathf.Min(slot.bindItem.itemData.current_stack, consume);
                    slot.bindItem.itemData.current_stack -= dec;
                    slot.SetItemAmount(slot.bindItem.itemData.current_stack);

                    consume -= dec;
                }
            }
        }

        //如果工具栏物品数量不够减， 检查背包
        if (consume != 0)
        {
            foreach (var slot in backpack.slotList)
            {
                if (consume == 0)
                {
                    break;
                }

                if (slot.bindItem != null && slot.bindItem.itemData.id == command.item_id)
                {
                    int dec = Mathf.Min(slot.bindItem.itemData.current_stack, consume);
                    slot.bindItem.itemData.current_stack -= dec;
                    slot.SetItemAmount(slot.bindItem.itemData.current_stack);

                    consume -= dec;
                }
            }
        }

        return CommandResult.OK();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ItemQueryRequestEvent>(OnItemQueryRequestEvent);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemQueryRequestEvent>(OnItemQueryRequestEvent);
    }

    private void OnItemQueryRequestEvent(ItemQueryRequestEvent @event)
    {
        int count = GetTotalItemCount(@event.item_id);

        @event.callBack?.Invoke(count);
    }

    private int GetTotalItemCount(int id)
    {
        int count = 0;

        foreach (var slot in toolBar.slotList)
        {
            if (slot.bindItem != null && slot.bindItem.itemData.id == id)
            {
                count += slot.bindItem.itemData.current_stack;
            }
        }

        foreach (var slot in backpack.slotList)
        {
            if (slot.bindItem != null && slot.bindItem.itemData.id == id)
            {
                count += slot.bindItem.itemData.current_stack;
            }
        }

        return count;
    }
}
