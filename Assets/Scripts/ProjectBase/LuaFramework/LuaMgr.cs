using System;
using UnityEngine;
using XLua;

/// <summary>
/// Lua 环境管理器（桥接层）。
/// 职责：持有 LuaEnv、注册加载器、以及 C#&lt;-&gt;Lua 之间的入口方法。
/// 注意：本类只做「桥接」，不承载业务逻辑；业务逻辑写在 Lua 脚本里。
/// </summary>
public class LuaMgr
{
    private static LuaMgr _instance;
    public static LuaMgr Instance
    {
        get { if (_instance == null) _instance = new LuaMgr(); return _instance; }
    }

    private LuaEnv _env;
    private LuaTable _monsterAI;   // require 'monster_ai' 返回的类表（含 new）
    private bool _inited;

    /// <summary>热更 AB 的 url 前缀。未接 AB 时保持空，走 Resources 回退。</summary>
    public static string LuaAbRoot = "";

    private LuaMgr() { }

    public void Init()
    {
        if (_inited) return;
        _inited = true;
        _env = new LuaEnv();
        _env.AddLoader(LoadLuaBytes);
        object[] ret = _env.DoString("return require 'monster_ai'");
        _monsterAI = (ret != null && ret.Length > 0) ? ret[0] as LuaTable : null;
        if (_monsterAI == null)
            Debug.LogError("[LuaMgr] require 'monster_ai' 失败，请检查 Assets/Resources/Lua/monster_ai.lua.txt");
    }

    private byte[] LoadLuaBytes(ref string filepath)
    {
        string module = filepath.Replace('.', '/');
        try
        {
            var res = AssetBundleFramework.ResourceManager.instance.Load(LuaAbRoot + module + ".lua.txt", false);
            if (res != null)
            {
                var ta = res.GetAsset<TextAsset>();
                if (ta != null && ta.bytes != null && ta.bytes.Length > 0) return ta.bytes;
            }
        }
        catch (Exception e) { Debug.LogWarning("[LuaMgr] AB 加载失败，回退 Resources：" + e.Message); }

        TextAsset text = Resources.Load<TextAsset>("Lua/" + module + ".lua");
        if (text == null) text = Resources.Load<TextAsset>("Lua/" + module);
        return text != null ? text.bytes : null;
    }

    // —— 怪物 AI 桥接（C# -> Lua）——

    public LuaTable CreateMonsterAI(EnemyBase enemy)
    {
        Init();
        if (_monsterAI == null) return null;
        LuaFunction create = _monsterAI.Get<LuaFunction>("new");
        object[] ret = create.Call(enemy);
        return (ret != null && ret.Length > 0) ? ret[0] as LuaTable : null;
    }

    public void TickMonsterAI(LuaTable ai, float dt)
    {
        if (ai == null) return;
        ai.Get<LuaFunction>("update")?.Call(ai, dt);
    }

    public void MonsterOnHurt(LuaTable ai, float damage)
    {
        if (ai == null) return;
        ai.Get<LuaFunction>("on_damage")?.Call(ai, damage);
    }

    public void MonsterOnDead(LuaTable ai)
    {
        if (ai == null) return;
        ai.Get<LuaFunction>("on_dead")?.Call(ai);
    }

    public void Dispose()
    {
        if (_env != null) { _env.Dispose(); _env = null; }
        _inited = false;
    }
}
