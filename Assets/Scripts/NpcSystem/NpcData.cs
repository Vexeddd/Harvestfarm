using System;
using UnityEngine;

/// <summary>
/// npc数据
/// </summary>
///
[Serializable]
public class NpcData
{
    public int id;
    public string npcId;

    public string name;

    public string prefab_name; //预制体名称
    public NpcType npc_type; //枚举类
    public Vector3 location; //出生位置
}