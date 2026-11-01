using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class JenkinsBuild
{
    // Jenkins 调用：-executeMethod JenkinsBuild.BuildWindows
    public static void BuildWindows()
    {
        // 1. 用 Build Settings 里勾选好的场景
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
            scenes[i] = EditorBuildSettings.scenes[i].path;

        if (scenes.Length == 0)
        {
            Debug.LogError("Build Settings 里没有场景！先到 File -> Build Settings 添加。");
            EditorApplication.Exit(1);
            return;
        }

        // 2. 输出路径：Builds/Windows/HarvestFarm.exe（Unity 会自动生成 _Data 等文件夹）
        string outDir = "Builds/Windows";
        System.IO.Directory.CreateDirectory(outDir);
        string outPath = System.IO.Path.Combine(outDir, "HarvestFarm.exe");

        // 3. 打 Windows 包
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("打包成功: " + outPath);
        }
        else
        {
            Debug.LogError("打包失败: " + report.summary.result);
            EditorApplication.Exit(1); // 非0 → Jenkins 判定失败
        }
    }
}
