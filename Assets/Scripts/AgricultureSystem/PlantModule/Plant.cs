using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plant : MonoBehaviour, IInteractalbe
{
    public PlantData plantData;
    public FarmlandTile bindTile;

    public bool isGrowing;
    public bool isMature;

    private float timePerStage;
    private float growthTimer = 0f;

    private int currentStage;
    public GameObject[] states;

    private void Awake()
    {
        states = GetComponentsInChildren<Transform>(true).Where(t => t != this.transform).Select(t => t.gameObject).ToArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        var water = bindTile.CheckWater();
        if (water)
        {
            StartGrow();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGrowing || isMature)
        {
            return;
        }

        if (growthTimer < timePerStage)
        {
            growthTimer += Time.deltaTime;
            if (growthTimer >= timePerStage)
            {
                growthTimer = 0f;
                SetGrowthStage(currentStage + 1);
            }
        }
    }


    public void BindTile(FarmlandTile tile)
    {
        bindTile = tile;
    }

    public void SetData(PlantData plantData)
    {
        this.plantData = plantData;
    }



    public void SetGrowthStage(int i)
    {
        currentStage = i;

        //设置生长时间
        if (currentStage <= plantData.timePerStage.Count)
        {
            timePerStage = plantData.timePerStage[currentStage - 1];
        }
        else
        {
            Mature();
        }

        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        int i = currentStage;

        if (currentStage - 1 < states.Length)
        {
            for (int j = 0; j < states.Length; j++)
            {
                states[j].SetActive(false);
                if (j == i - 1)
                {
                    states[j].SetActive(true);
                }
            }
        }
    }



    public void StartGrow()
    {
        isGrowing = true;
    }

    public void Mature()
    {
        isMature = true;
    }

    private void Harvest()
    {
        //土地变干
        bindTile.Harvest();
        this.bindTile = null;

        //收获物品逻辑
        ItemData itemData = LoadManager.Instance.itemDataDic[this.plantData.harvest_id];

        var harvestItem = new ItemData(itemData);
        harvestItem.current_stack = this.plantData.harvest_amount;
        InventorySystem.Instance.CreatItemInWorld(harvestItem, this.transform.position);

        //上报任务
        QuestManager.Instance.ReportAction(QuestType.HarvestCrop, plantData.harvest_id.ToString(), 1);

        //播放音效
        MusicManager.Instance.PlayOneShot(MusicType.Harvest);

        //销毁
        Destroy(gameObject);
    }


    public string GetInteractTip()
    {
        return $"按F收获 {this.plantData.harvest_Item}";
    }

    public bool CanInteract()
    {
        return isMature;
    }

    public void Ineract(GameObject obj)
    {
        Harvest();
    }

}
