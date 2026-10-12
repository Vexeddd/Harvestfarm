using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffModule : MonoBehaviour
{
    public Dictionary<int, List<BuffData>> buffDic = new Dictionary<int, List<BuffData>>();

    public readonly List<BaseBuff> buffs = new List<BaseBuff>();

    void Awake()
    {
        List<BuffData> lbd = JsonConvert.DeserializeObject<List<BuffData>>(Resources.Load<TextAsset>("Data/buff").text);

        foreach (BuffData data in lbd)
        {
            if (buffDic.TryGetValue(data.item_id, out var buffs))
            {
                if (!buffs.Any(b => b.id == data.id))
                {
                    buffs.Add(data);
                }
            }
            else
            {
                var bl = new List<BuffData>();
                bl.Add(data);
                buffDic.Add(data.item_id, bl);
            }
        }
    }

    private void Start()
    {
        BuffData decSat = new BuffData();
        decSat.buffType = BuffType.Forever;
        decSat.vitalType = VitalType.Satiety;
        decSat.value = -0.5f;

        AddBuff(decSat);
    }

    private float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1)
        {
            timer = 0;
            UpDateBuff();
        }

    }

    private void UpDateBuff()
    {
        if (buffs.Count <= 0)
        {
            return;
        }

        for (int i = 0; i < buffs.Count; i++)
        {
            bool isFinal = buffs[i].OnUpdate(1);
            if (isFinal)
            {
                buffs[i].OnRemoved();
                buffs.RemoveAt(i);
            }
        }
    }


    public void OnEat(ItemData itemData)
    {
        if (buffDic.TryGetValue(itemData.id, out var buffDatas))
        {
            foreach (var buff in buffDatas)
            {
                AddBuff(buff);
            }
        }
    }

    public void AddBuff(BuffData buff)
    {
        BaseBuff bf = BuffFactory.CreatBaseBuff(buff);
        bf.Apply();
        buffs.Add(bf);
    }

}
