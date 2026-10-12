using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveManager : SingletonAutoMono<SaveManager>
{
    protected override string RootNodeName => "[Manager]";

    private readonly List<ISaveable> _saveables = new List<ISaveable>();

    private static string SavePath => Path.Combine(Application.persistentDataPath, "save_slot_1.json");

    protected override void Awake()
    {
        base.Awake();

        // Register systems in save/load order. Add more (quest, plant, chicken...) in later phases.
        _saveables.Add(new PlayerSaveable());
        _saveables.Add(new VitalSaveable());
        _saveables.Add(new InventorySaveable());
        _saveables.Add(new PlotSaveable());
    }

    private void Update()
    {
        // Temporary test keys until a save UI exists.
        if (Input.GetKeyDown(KeyCode.F5)) Save();
        if (Input.GetKeyDown(KeyCode.F9)) Load();
    }

    public void Save()
    {
        var data = new SaveData();
        foreach (var s in _saveables) s.OnSave(data);

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(SavePath, json);
        Debug.Log("[Save] saved to " + SavePath);
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("[Save] no save file at " + SavePath);
            return;
        }

        string json = File.ReadAllText(SavePath);
        var data = JsonConvert.DeserializeObject<SaveData>(json);
        foreach (var s in _saveables) s.OnLoad(data);
        Debug.Log("[Save] loaded from " + SavePath);
    }

    // Ensure the manager (and its F5/F9 handling) exists without touching the scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AutoInit()
    {
        _ = Instance;
    }
}
