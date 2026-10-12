/// <summary>
/// 任务类型
/// </summary>
public enum QuestType
{
    None = 0,                 // 无类型，占位用

    // ===== 农业 / 生存 =====
    PlantCrop,                // 种植作物
    HarvestCrop,              // 收获作物

    // ===== 物品相关 =====
    CollectItem,              // 收集物品（进背包）
    ConsumeItem,              // 消耗物品
    CraftItem,                // 制作物品
    BuyItem,                  // 购买物品
    SellItem,                 // 出售物品

    // ===== 建筑 =====
    PlaceBuilding,            // 放置建筑
    UpgradeBuilding,          // 升级建筑
    InteractBuilding,         // 与建筑互动

    // ===== 战斗 =====
    KillMonster,              // 击杀怪物
    DamageMonster,            // 对怪物造成伤害

    // ===== 探索 =====
    ReachArea,                // 到达某个区域
    DiscoverLocation,         // 解锁地点

    // ===== NPC / 社交 =====
    TalkToNPC,                // 与 NPC 对话
    NPCFavorLevel,            // NPC 好感度达到某等级

    // ===== 成长 =====
    PlayerLevel,              // 玩家等级达标
    SkillLevel,               // 技能熟练度达标

    // ===== 剧情控制 =====
    TriggerStoryFlag          // 触发剧情标记（高级用）
}

//方便任务上报 种植5个萝卜  报告(PlantCrop, id, 1)
// 击杀5个 xx怪物         报告(KillMonster, id, 1)