using System;
using System.Collections.Generic;

/// <summary>
/// Pure-data snapshot of everything persisted in a save file.
/// No UnityEngine object references in here (so it can round-trip through JSON).
/// </summary>
[Serializable]
public class SaveData
{
    // player
    public float px;
    public float py;
    public float pz;

    // vitals
    public float hp;
    public float stamina;
    public float satiety;

    // inventory containers (slot order preserved; null slot entry = empty)
    public List<ContainerSave> containers = new List<ContainerSave>();

    // story / quests
    public PlotSave plot;
}

[Serializable]
public class ContainerSave
{
    public string name;
    public List<ItemStackSave> slots = new List<ItemStackSave>();
}

[Serializable]
public class ItemStackSave
{
    public int id;
    public int count;
    public string uuid;            // stateful items (tools/weapons) only
    public RuntimeStateSave state; // tool/weapon runtime state, null if never equipped
}

[Serializable]
public class RuntimeStateSave
{
    public float durability; // tool: current_duration ; weapon: current_duability
    public int ammo;         // weapon current ammo
    public int water;        // tool watering can current water
}

[Serializable]
public class PlotSave
{
    public int currentSegment; // index of the segment the runner is on
    public int currentNode;    // node index the runner is blocked on (-1 = no runner)
    public List<QuestSave> quests = new List<QuestSave>();
}

[Serializable]
public class QuestSave
{
    public string quest_id;
    public string title;
    public bool isComplete;
    public List<StepSave> steps = new List<StepSave>();
}

[Serializable]
public class StepSave
{
    public string type; // QuestType name
    public string target_id;
    public int required_count;
    public int current_count;
    public string desc;
    public bool isCompleted;
}
