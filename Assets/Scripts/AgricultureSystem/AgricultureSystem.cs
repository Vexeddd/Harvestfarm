using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgricultureSystem : SingletonAutoMono<AgricultureSystem>
{
    public GameObject soil_prefab; //土地预制体

    public ToolModule toolModule;

    public PlantModule plantModule;

    protected override void Awake()
    {
        base.Awake();

        toolModule = UnityEngine.Object.FindObjectOfType<ToolModule>();
        if (toolModule == null)
        {
            GameObject obj1 = new GameObject("ToolModule");
            toolModule = obj1.AddComponent<ToolModule>();
            obj1.transform.parent = this.transform;
        }

        plantModule = UnityEngine.Object.FindObjectOfType<PlantModule>();
        if (plantModule == null)
        {
            GameObject obj2 = new GameObject("PlantModule");
            plantModule = obj2.AddComponent<PlantModule>();
            obj2.transform.parent = this.transform;
        }



        soil_prefab = Resources.Load<GameObject>("Prefab/Soil");

    }
}
