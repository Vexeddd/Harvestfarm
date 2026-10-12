using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmlandTile : MonoBehaviour, IToolInteractable
{
    public Vector2Int gridPos;
    public Vector3 worldPos;

    public GameObject _dry;
    public GameObject _plow;
    public GameObject _water;
    public GameObject _water_plow;

    public SoliData soliData;
    public Plant bindPlant;

    private void Awake()
    {
        soliData = new SoliData();

        _dry = transform.Find("Dry").gameObject;
        _plow = transform.Find("Plow").gameObject;
        _water = transform.Find("Water").gameObject;
        _water_plow = transform.Find("Water_plow").gameObject;

        Dry();
    }


    public ToolUseResolt OnToolUsed(ToolContext context)
    {
        //开垦
        if (context.toolData.tool_type == ToolType.Hoe)
        {
            bool plowRes = TryPlow();
            if (!plowRes)
            {
                return ToolUseResolt.None;
            }

            return ToolUseResolt.Success;
        }
        //浇水
        else if (context.toolData.tool_type == ToolType.WaterCan)
        {
            bool waterRes = TryWater();
            if (!waterRes)
            {
                return ToolUseResolt.None;
            }

            return ToolUseResolt.Success;
        }
        else
        {
            return ToolUseResolt.None;
        }
    }


    private bool TryPlow()
    {
        if (soliData.isPlowed || soliData.isPlanted)
        {
            return false;
        }
        Plow();
        return true;
    }
    private bool TryWater()
    {
        if (!soliData.isPlowed || soliData.isWatered)
        {
            return false;
        }
        WaterPlow();

        return true;
    }


    private void Dry()
    {
        _dry.SetActive(true);
        _plow.SetActive(false);
        _water_plow.SetActive(false);

        _water.SetActive(false);
    }

    private void Plow()
    {
        _dry.SetActive(false);
        _plow.SetActive(true);
        _water_plow.SetActive(false);

        _water.SetActive(false);

        soliData.isPlowed = true;   
    }

    private void WaterPlow()
    {
        //播放音效
        MusicManager.Instance.PlayOneShot(MusicType.Water);

        //土地状态更变
        _dry.SetActive(false);
        _plow.SetActive(false);
        _water_plow.SetActive(true);

        _water.SetActive(false);

        //数据记录
        soliData.isWatered = true;

        //通知植物
        if (bindPlant != null)
        {
            bindPlant.StartGrow();
        }
    }

    public bool CanPlant()
    {
        bool res = (!soliData.isPlanted) && soliData.isPlowed;
        return res;
    }

    public void BindPlant(Plant plant)
    {
        this.bindPlant = plant;
    }

    public bool CheckWater()
    {
        return soliData.isWatered;
    }

    public void Harvest()
    {
        soliData.isWatered = false;
        soliData.isPlanted = false;
        soliData.isPlowed = false;

        Dry();
        this.bindPlant = null;
    }
}
