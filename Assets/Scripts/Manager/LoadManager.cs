using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadManager : SingletonAutoMono<LoadManager>
{
    protected override string RootNodeName => "[Manager]";

    //ItemData
    public List<ItemData> itemDatas = new List<ItemData>();
    public Dictionary<int, ItemData>    itemDataDic = new Dictionary<int, ItemData>();
    public Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();

    //ToolData
    public List<ToolData> toolDatas = new List<ToolData>();
    public Dictionary<int, ToolData> toolDataDic = new Dictionary<int, ToolData>();

    //MappingData
    public Dictionary<int, string> mappingPathDic = new Dictionary<int, string>();
    public Dictionary<int, GameObject>mappingPrefabDic = new Dictionary<int, GameObject>();

    //PlantAbout
    public Dictionary<SeedType, GameObject> seedPrefabDic = new Dictionary<SeedType, GameObject>();
    public Dictionary<SeedType, PlantData> plantDataDic = new Dictionary<SeedType, PlantData>();
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        LoadItemData();
        LoadToolData();
        LoadMappingData();
        LoadSeedData();
    }

    private void LoadItemData()
    {
        TextAsset data = ResMgr.Instance.Load<TextAsset>("Data/ItemData");
        itemDatas = JsonConvert.DeserializeObject<List<ItemData>>(data.text);

        InitAtlas();
        foreach (var item in itemDatas)
        {
            item.icon = spriteDic.ContainsKey(item.icon_name) ? spriteDic[item.icon_name] : null;

            itemDataDic.Add(item.id, item);
        }
    }

    private void InitAtlas()
    {
        Sprite[] item = Resources.LoadAll<Sprite>("Image/UI/springobjects");
        foreach (var sprite in item)
        {
            //Ìí¼Óµ½Í¼Æ¬×Öµä
            spriteDic.Add(sprite.name, sprite);
        }

        Sprite[] item2 = Resources.LoadAll<Sprite>("Image/UI/tools");
        foreach (var sprite in item2)
        {
            //Ìí¼Óµ½Í¼Æ¬×Öµä
            spriteDic.Add(sprite.name, sprite);
        }

        Sprite[] item3 = Resources.LoadAll<Sprite>("Image/Other");
        foreach (var sprite in item3)
        {
            //Ìí¼Óµ½Í¼Æ¬×Öµä
            spriteDic.Add(sprite.name, sprite);
        }

        Debug.Log("¼ÓÔØÍ¼Æ¬" + spriteDic.Count);
    }

    private void LoadToolData()
    {
        TextAsset text = ResMgr.Instance.Load<TextAsset>("Data/tool");
        toolDatas = JsonConvert.DeserializeObject<List<ToolData>>(text.text);

        foreach (var data in toolDatas)
        {
            toolDataDic.Add(data.item_id, data);
        }
    }

    private void LoadMappingData()
    {
        TextAsset mappingData = ResMgr.Instance.Load<TextAsset>("Data/PrefabMapping");
        mappingPathDic = JsonConvert.DeserializeObject<Dictionary<int, string>>(mappingData.text);
        foreach (var item in mappingPathDic)
        {
            mappingPrefabDic.Add(item.Key, Resources.Load<GameObject>(item.Value));
        }
    }

    private void LoadSeedData()
    {
        GameObject[] seeds = Resources.LoadAll<GameObject>("Prefab/Plant");
        foreach (var seed in seeds)
        {
            seedPrefabDic.Add(Enum.Parse<SeedType>(seed.name), seed);
        }

        TextAsset text = Resources.Load<TextAsset>("Data/PlantData");
        var plantDataList = JsonConvert.DeserializeObject<List<PlantData>>(text.text);
        foreach (PlantData plantData in plantDataList)
        {
            plantDataDic.Add(plantData.seed_type, plantData);
        }
    }



    internal GameObject GetMappingPrefab(int id)
    {
        if (mappingPrefabDic.ContainsKey(id))
        {
            return mappingPrefabDic[id];
        }
        return null;
    }
}
