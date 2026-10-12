using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSystem : SingletonAutoMono<NpcSystem>
{
    public Dictionary<NpcType, NpcData> npc_dataDic = new Dictionary<NpcType, NpcData>();

    public Dictionary<NpcType, GameObject> npc_prefabDic = new Dictionary<NpcType, GameObject>();

    public Dictionary<NpcType, NpcController> npc_instanceDic = new Dictionary<NpcType, NpcController>();


    public Dictionary<string, NpcRegisterDialogEvent> plot_dialog = new Dictionary<string, NpcRegisterDialogEvent>();


    public GameObject OptionBlock_prefab;

    protected override void Awake()
    {
        base.Awake();

        List<NpcData> datas = JsonConvert.DeserializeObject<List<NpcData>>(Resources.Load<TextAsset>("Data/Npc").text);
        foreach (var data in datas)
        {
            npc_dataDic.Add(data.npc_type, data);

            GameObject go = Resources.Load<GameObject>("Prefab/NPC/" + data.prefab_name);
            npc_prefabDic.Add(data.npc_type, go);

            CreateNPC(data, go);
        }

        OptionBlock_prefab = Resources.Load<GameObject>("Prefab/UI/OptionBlock");

    }

    public void CreateNPC(NpcData data, GameObject go)
    {
        GameObject npcIns = GameObject.Instantiate(go, data.location, Quaternion.identity);

        NpcController controller = npcIns.GetComponent<NpcController>();
        if (controller == null)
        {
            controller = npcIns.AddComponent<NpcController>();
        }

        controller.Init(data);

        npc_instanceDic.Add(data.npc_type, controller);
    }



    private void OnEnable()
    {
        EventBus.Subscribe<NpcRegisterDialogEvent>(OnNpcRegisterDialogHandle);
    }


    private void OnDisable()
    {
        EventBus.Unsubscribe<NpcRegisterDialogEvent>(OnNpcRegisterDialogHandle);
    }


    private void OnNpcRegisterDialogHandle(NpcRegisterDialogEvent @event)
    {
        plot_dialog.Add(@event.data.npc, @event);
    }
}
