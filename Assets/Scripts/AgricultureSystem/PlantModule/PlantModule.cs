using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class PlantModule : MonoBehaviour
{
    //监听工具栏切换  种子……
    public SeedType currentSeed = SeedType.None;

    //管理土地网格 土地快， 浇水 开垦..
    private float seedinteract_distance = 8f;    //种子交互距离
    //生成种子 种植植物  收获

    public List<FarmlandGrid> girdList = new List<FarmlandGrid>();

    private ItemData currentItemData;


    private void OnEnable()
    {
        EventBus.Subscribe<ToolbarItemChangedEvent>(OnToolbarItemChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ToolbarItemChangedEvent>(OnToolbarItemChanged);
    }


    private void Start()
    {
        FarmlandGrid grid1 = new FarmlandGrid();
        grid1.CreateFarmTile(new Vector3(-26.5f, 0.12f, 18), 3, 5);
        girdList.Add(grid1);
    }

    private void Update()
    {
        if (currentSeed != SeedType.None)   
        {
            if (Input.GetMouseButtonDown(0))
            {
                bool res = TryPlantSeed();
            }
        }
    }


    private void OnToolbarItemChanged(ToolbarItemChangedEvent e)
    {
        //if (e.inventoryItem != null && e.inventoryItem.itemData.item_type == ItemType.Seed)
        //{
        //    currentSeed = Enum.Parse<SeedType>(e.inventoryItem.itemData.item_sub_type.ToString());
        //    this.currentItemData = e.inventoryItem.itemData;
        //}
        //else
        //{
        //    currentSeed = SeedType.None;
        //    this.currentItemData = e.inventoryItem.itemData;
        //}
        var item = e.inventoryItem;
        if (item == null || item.itemData == null)
        {
            currentSeed = SeedType.None;
            currentItemData = null;
            return;
        }
        if (item.itemData.item_type == ItemType.Seed)
        {
            currentSeed = Enum.Parse<SeedType>(item.itemData.item_sub_type.ToString());
            currentItemData = item.itemData;
        }
        else
        {
            currentSeed = SeedType.None;
            currentItemData = item.itemData;
        }
    }

    private bool TryPlantSeed()
    {
        Debug.Log("使用" + currentSeed.ToString());
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, seedinteract_distance, LayerMask.GetMask("Farmland")))
        {
            FarmlandTile tile = hit.collider.GetComponent<FarmlandTile>();
            if (tile.CanPlant())
            {
                PlantSeed(tile);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void PlantSeed(FarmlandTile tile)
    {
        //拿到预制体
        GameObject seedPrefab = LoadManager.Instance.seedPrefabDic[currentSeed];

        //拿到生成的位置
        Vector3 pos = tile.transform.position + new Vector3(0, 0.2f, 0);
        //生成植物预制体在位置上
        GameObject go = GameObject.Instantiate(seedPrefab, pos, Quaternion.identity);
        Plant plant = go.GetComponent<Plant>();
        if (plant == null)
        {
            plant = go.AddComponent<Plant>();
        }

        //土地标记已种植
        tile.soliData.isPlanted = true;

        //查询设置植物数据
        PlantData plantData = LoadManager.Instance.plantDataDic[currentSeed];
        plant.SetData(plantData);
        plant.SetGrowthStage(1);

        //土地关联植物
        tile.BindPlant(plant);
        plant.BindTile(tile);

        if (currentItemData.current_stack == 1)
        {
            currentSeed = SeedType.None;
        }
        //背包种子-1
        CommandBus.Send(new CommandReduceItemAmount { amount = 1});

        //任务上报
        QuestManager.Instance.ReportAction(QuestType.PlantCrop, plantData.item_id.ToString(), 1);

        //播放音效
        MusicManager.Instance.PlayOneShot(MusicType.PlantSeed);
    }

}
