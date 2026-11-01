using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 一键批量缩小贴图导入尺寸（Max Size）。
/// 只降不升；规则在下面 Rules 里按需改。
/// 运行：菜单 Tools → 一键批量缩贴图。
/// 说明：改的是“导入设置里的 Max Size”，不修改原始贴图文件，随时可以在 Inspector 里改回来。
/// </summary>
public static class BatchTextureResizer
{
    // 规则：路径包含 folderKeyword 的贴图，Max Size 缩到 targetMaxSize。
    // 从上到下匹配，先命中先用；targetMaxSize 会自动吸附到 2 的幂（32/64/128/256/512/1024/2048/4096/8192）。
    private static readonly Rule[] Rules =
    {
        new Rule("AssetBundle/Icon", 256),       // 物品图标
        new Rule("AK STUDIO ART",    512),       // 相机 / 三脚架等道具
        new Rule("Resources/Image",  512),       // UI 图片
        new Rule("Crosshairs",       512),       // 准星
        new Rule("PolygonFantasyRivals", 1024),  // 角色
        new Rule("PolygonFarm",      1024),      // 农场环境
        new Rule("Kevin Iglesias",   1024),      // 角色动画
        new Rule("Bullets",          1024),      // 子弹等特效贴图
    };

    // 全局上限：所有贴图一律不超过这个尺寸（0 = 不设全局上限）。用来兜底处理 5805、4096 这类超大贴图。
    private const int GlobalCap = 2048;

    private struct Rule
    {
        public string folderKeyword;
        public int targetMaxSize;
        public Rule(string keyword, int size) { folderKeyword = keyword; targetMaxSize = size; }
    }

    private struct Change
    {
        public string path;
        public int from;
        public int to;
        public Change(string p, int f, int t) { path = p; from = f; to = t; }
    }

    // ===== 备份 / 还原：缩小前把「原始 maxTextureSize」记到 JSON，出错可一键还原 =====

    [Serializable]
    private class TextureSizeRecord
    {
        public string path;
        public int originalMaxSize;
    }

    [Serializable]
    private class BackupData
    {
        public List<TextureSizeRecord> records = new List<TextureSizeRecord>();
    }

    // 备份文件放在项目根目录（Assets 的上一级），Unity 不会把它当资源导入
    private static string BackupPath
    {
        get { return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "TextureSizeBackup.json")); }
    }

    private static BackupData LoadBackup()
    {
        if (!File.Exists(BackupPath)) return new BackupData();
        try
        {
            return JsonUtility.FromJson<BackupData>(File.ReadAllText(BackupPath)) ?? new BackupData();
        }
        catch
        {
            return new BackupData();
        }
    }

    private static void SaveBackup(BackupData data)
    {
        File.WriteAllText(BackupPath, JsonUtility.ToJson(data, true));
    }

    [MenuItem("Tools/一键批量缩贴图")]
    public static void Run()
    {
        // 收集所有需要缩小的贴图
        var changes = new List<Change>();
        foreach (string guid in AssetDatabase.FindAssets("t:Texture"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            int current = importer.maxTextureSize;
            int target = GetTarget(path);
            if (target > 0 && current > target)
            {
                changes.Add(new Change(path, current, target));
            }
        }

        if (changes.Count == 0)
        {
            EditorUtility.DisplayDialog("一键批量缩贴图", "没有需要缩小的贴图。", "确定");
            return;
        }

        if (!EditorUtility.DisplayDialog("一键批量缩贴图", BuildSummary(changes), "缩小", "取消"))
            return;

        // 应用前先把「真正的原始尺寸」记进备份，供“还原”使用。
        // 已在备份里的保留不动，避免多次运行后把「原始值」覆盖成「已缩小的值」。
        var backup = LoadBackup();
        var known = new HashSet<string>();
        foreach (var r in backup.records) known.Add(r.path);
        foreach (var c in changes)
        {
            if (!known.Contains(c.path))
            {
                backup.records.Add(new TextureSizeRecord { path = c.path, originalMaxSize = c.from });
                known.Add(c.path);
            }
        }
        SaveBackup(backup);

        int applied = 0;
        foreach (var c in changes)
        {
            var importer = AssetImporter.GetAtPath(c.path) as TextureImporter;
            if (importer == null) continue;
            importer.maxTextureSize = c.to;
            importer.SaveAndReimport();
            applied++;
        }

        AssetDatabase.SaveAssets();

        foreach (var c in changes)
            Debug.Log($"[缩贴图] {c.from} → {c.to}  {c.path}");

        Debug.Log($"<color=#7CFC00>[缩贴图] 完成：共缩小 {applied} 张贴图。</color>");
    }

    [MenuItem("Tools/还原贴图尺寸")]
    public static void Restore()
    {
        var backup = LoadBackup();
        if (backup.records.Count == 0)
        {
            EditorUtility.DisplayDialog("还原贴图尺寸", "没有找到备份记录，无法还原。", "确定");
            return;
        }

        // 只还原「当前尺寸和原始尺寸不一样」的贴图，避免无谓的重导入
        var toRestore = new List<Change>();
        foreach (var r in backup.records)
        {
            var importer = AssetImporter.GetAtPath(r.path) as TextureImporter;
            if (importer == null) continue;
            if (importer.maxTextureSize != r.originalMaxSize)
                toRestore.Add(new Change(r.path, importer.maxTextureSize, r.originalMaxSize));
        }

        if (toRestore.Count == 0)
        {
            EditorUtility.DisplayDialog("还原贴图尺寸", "所有贴图已经是原始尺寸，无需还原。", "确定");
            return;
        }

        var sb = new System.Text.StringBuilder();
        sb.Append("将还原 ").Append(toRestore.Count).AppendLine(" 张贴图的 Max Size：");
        sb.AppendLine();
        var groups = new Dictionary<string, int>();
        foreach (var c in toRestore)
        {
            string key = c.from + " → " + c.to;
            groups[key] = groups.TryGetValue(key, out int n) ? n + 1 : 1;
        }
        foreach (var kv in groups)
            sb.Append("  ").Append(kv.Key).Append("  × ").AppendLine(kv.Value.ToString());
        sb.AppendLine();
        sb.Append("是否还原？");

        if (!EditorUtility.DisplayDialog("还原贴图尺寸", sb.ToString(), "还原", "取消"))
            return;

        int count = 0;
        foreach (var c in toRestore)
        {
            var importer = AssetImporter.GetAtPath(c.path) as TextureImporter;
            if (importer == null) continue;
            importer.maxTextureSize = c.to;
            importer.SaveAndReimport();
            count++;
        }
        AssetDatabase.SaveAssets();

        Debug.Log($"<color=#7CFC00>[还原贴图] 完成：共还原 {count} 张贴图到原始尺寸。</color>");
        Debug.Log("[还原贴图] 备份文件在项目根目录 TextureSizeBackup.json，可反复还原；不再需要可手动删除。");
    }

    private static int GetTarget(string path)
    {
        int target = GlobalCap;
        foreach (var rule in Rules)
        {
            if (path.Contains(rule.folderKeyword))
            {
                target = rule.targetMaxSize;
                break;
            }
        }
        if (target <= 0) return 0;
        return SnapToPowerOfTwo(target);
    }

    // 吸附到 2 的幂（向下取，最小 32）
    private static int SnapToPowerOfTwo(int v)
    {
        if (v <= 32) return 32;
        int p = 32;
        while (p * 2 <= v) p *= 2;
        return p;
    }

    private static string BuildSummary(List<Change> changes)
    {
        // 按“原尺寸 → 目标尺寸”汇总数量，对话框里更直观
        var groups = new Dictionary<string, int>();
        foreach (var c in changes)
        {
            string key = c.from + " → " + c.to;
            groups[key] = groups.TryGetValue(key, out int n) ? n + 1 : 1;
        }

        var sb = new System.Text.StringBuilder();
        sb.Append("将缩小 ").Append(changes.Count).AppendLine(" 张贴图的 Max Size：");
        sb.AppendLine();
        foreach (var kv in groups)
            sb.Append("  ").Append(kv.Key).Append("  × ").AppendLine(kv.Value.ToString());
        sb.AppendLine();
        sb.AppendLine("改的是导入设置里的 Max Size，不碰原始贴图文件，可随时改回。");
        sb.Append("是否继续？");
        return sb.ToString();
    }
}
