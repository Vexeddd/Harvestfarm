# HarvestFarm 项目文档

> 求职用 3D 农场经营游戏 Demo（类《星露谷物语》）：种植、养殖、采集、工具、烹饪、商店、背包、任务剧情、战斗、载具。
> 本文档是当前代码的真实快照（2026-09），后续每次改动请同步更新。

## 项目概览

- **类型**：3D 农场经营 + 轻生存/剧情（末日农场题材）
- **引擎**：Unity 2022.3.62f3（`ProjectSettings/ProjectVersion.txt`）
- **渲染管线**：内置渲染管线（Built-in RP，未用 URP/HDRP）
- **输入方案**：旧版 Input Manager（`Input.GetKeyDown/GetAxis`，未引入 Input System 包）
- **UI 文本**：TextMeshPro 3.0.7；**序列化**：Newtonsoft.Json 3.2.2
- **其他包**：Timeline 1.7.7、Visual Scripting 1.9.4、ugui、video、tilemap 等（见 `Packages/manifest.json`）
- **程序集**：无 `.asmdef`，全部在 Assembly-CSharp
- **场景**：仅 `Assets/Scenes/SampleScene.unity`（其余 `*.unity` 均为美术/插件自带 Demo 场景）
- **版本控制**：目前不是 Git 仓库
- **代码规模**：`Assets/Scripts` 下约 150 个脚本、约 9400 行

## 目录结构

```
Assets/
├── Scripts/
│   ├── ProjectBase/            # 自研框架层（基础设施）
│   │   ├── Base/               #   单例 SingletonAutoMono/SingletonMono/BaseManager + Bootstrapper
│   │   ├── EventBus/           #   类型化事件总线 EventBus + 旧版 EventCenter + 事件类
│   │   ├── CommandBus/         #   命令总线（命令 + 处理器 + 具体命令）
│   │   ├── FSM/                #   StateMachine / IState（目前耦合怪物）
│   │   ├── Mono/               #   MonoMgr/MonoController（统一 Update 委托 + 协程宿主）
│   │   ├── Pool/               #   PoolMgr 对象池
│   │   ├── Reactive/           #   Reactive<T> 响应式属性（+ ReactiveTest 演示）
│   │   ├── Res/                #   ResMgr 资源加载（Resources 同步/异步）
│   │   ├── StateManager/       #   UUID→运行时状态（Item/Tool/Weapon）
│   │   └── StaticExpansion/    #   Transform 扩展（FindDirect/FindRecursive）
│   ├── Manager/                # LoadManager（配置表加载）/ InputContextManager / MusicManager / QuestManager
│   ├── Player/                 # 玩家：Player、PlayeMovement、PersonCamera、PlayerInteraction、PlayerVehicleController
│   ├── AgricultureSystem/      # 农业：种植(PlantModule) + 工具(ToolModule)
│   ├── AnimalSystem/           # 养殖：鸡舍、鸡的行为组件、鸡蛋
│   ├── InventorySystem/        # 背包：容器/槽位/物品 + 背包UI + 可交互实体(掉落/储物箱/水源)
│   ├── ShopSystem/             # 商店
│   ├── UISystem/               # UIMgr / UIContainer / InteractionUI
│   ├── PhysiologySystem/       # 生理：血/体力/饱食度 + Buff
│   ├── MonsterSystem/          # 怪物：EnemyBase + FSM 状态
│   ├── NpcSystem/              # NPC：数据/控制器/交互面板
│   ├── PlotSystem/             # 剧情：Story/Segment/Node 数据 + 对话/任务/影片/时间轴模块
│   ├── ProductionSystem/       # 生产：烹饪(CookingModule) + 建筑(ProductBuildingBase)
│   ├── VehicleSystem/          # 载具：上/下车、驾驶、座位
│   └── WeaponSystem/           # 武器：近战/远程、子弹、弹药UI
├── Resources/                  # 运行时加载（Resources.Load 从这读取）
│   ├── Data/                   #   实际生效的 JSON 配置
│   ├── Font/ Image/ Material/ Model/ Movie/ Music/ Texture/
│   ├── Prefab/                 #   运行时预制体（Animal/Bullet/Monster/NPC/Plant/Prop/UI/Weapon/Soil/Slot/Item…）
│   └── UIBase/
├── Data/                       # ⚠️ 与 Resources/Data 重复的一份（编辑器备用，运行时不用）
├── Art/                        # 美术资源（PolygonFarm、PolygonFantasyRivals、AK STUDIO ART…）
├── Models/                     # 农场模型源文件（fbx/mat）
├── BackUp/                     # 已废弃框架备份（Input/Mono/Music/PureMVC 等，不再编译）
└── Kevin Iglesias/ OccaSoftware/  # 第三方资源（动画/准星）
```

## 框架与架构

### 单例三件套
- **`SingletonAutoMono<T>`**：自动建 GameObject 挂 `[System]`/`[Manager]` 根节点并 `DontDestroyOnLoad`。业务系统用它（`RootNodeName` 可覆盖）。
- **`BaseManager<T>`**：纯 C# 单例（反射调私有构造）。`ResMgr`/`PoolMgr`/`MonoMgr`/`EventCenter` 用它。
- **`SingletonMono<T>`**：场景内普通组件单例（`Player`、`InteractionUI`、各 Panel 用静态 `_instance`）。

### 启动顺序（Bootstrapper）
`[RuntimeInitializeOnLoadMethod(AfterSceneLoad)]` 依次触摸 12 个系统单例，保证 Awake 先于 Start 完成：
`UIMgr → LoadManager → InventorySystem → AgricultureSystem → AnimalSystem → PhysiologySystem → ProductionSystem → WeaponSystem → MonsterSystem → NpcSystem → PlotSystem → VehicleSystem`

### 事件 / 命令（三套并存）
| 机制 | 类型 | 用途 |
|---|---|---|
| `EventBus`（推荐） | 静态泛型 `Subscribe<T>/Publish<T>` | ToolbarItemChanged、EatFood、VitalQuery、ItemQuery、NpcRegisterDialog |
| `EventCenter`（旧） | `BaseManager` + 字符串事件名 | 几乎不再使用 |
| `CommandBus` | 静态 `Register<T>/Send<T>` | 扣血/体力/饱食、扣物品、扣背包（命令与处理器解耦） |

### 数据加载（LoadManager.Awake）
1. `ItemData.json` → `itemDataDic`（按 id），图集 `Image/UI/springobjects`、`tools`、`Image/Other` 填充 `icon`。
2. `tool.json` → `toolDataDic`（按 `item_id`）。
3. `PrefabMapping.json` → `mappingPathDic` / `mappingPrefabDic`（物品 id → 掉落物预制体）。
4. `Prefab/Plant` 下所有预制体 + `PlantData.json` → `seedPrefabDic` / `plantDataDic`（按 `SeedType` 枚举）。

### 状态管理（StateManager）
- `RegisterState(uuid, state)` / `GetState<T>(uuid)`，用 UUID 给「有状态物品」挂运行时状态。
- `ToolRuntimeState`（耐久 current_duration / 水量 current_water）、`WeaponRuntimeState`（耐久 / 弹匣 current_ammo）。
- UUID 在 `InventorySystem.TryAddItem` 对 `has_state` 物品自动生成。

## 各系统速览

### 玩家 Player
- `PlayeMovement`：相机相对移动（WASD + Shift 冲刺 + Space 跳跃），`InputContext` 切换游戏/驾驶。
- `PersonCamera`：第三人称轨道相机（鼠标 + 滚轮缩放），`Cursor.lockState` 控制。
- `PlayerInteraction`：屏幕中心射线探测 `IInteractalbe`，F 键交互，`InteractionUI` 显示提示。

### 背包 InventorySystem
- 容器：快捷栏 10 / 背包 32 / 储物箱 8 / 手心(拖拽中转) 1。
- `InventoryItem` 拖拽（Begin/End Drag）+ `InventorySlot` 点击/合并/交换/拆分/右键放一个。
- `TryAddItem`：优先堆叠 → 空槽 → 掉落世界（`CreatItemInWorld`）。
- 可交互实体：`CollectItem`/`DropItem`（拾取）、`StorageboxEntity`（储物箱）、`WaterWall`（水壶装水）。

### 农业 AgricultureSystem
- **种植**：`ToolBar` 切到种子 → `PlantModule.OnToolbarItemChanged` 解析 `SeedType` → 左键 `TryPlantSeed` 射线命中 `FarmlandTile` → 实例化植物预制体，`Plant` 按 `PlantData.timePerStage` 分阶段生长，成熟后可 F 收获掉落果实。
- **工具**：`ToolModule` + `ToolInstance` + `ToolBehaviorFactory` → 锄/镐/斧/水壶 4 种行为（`BaseToolBehaviour` 统一射线命中 `IToolInteractable`）。
  - 锄头→耕地(FarmlandTile)、水壶→浇水(需先装水)、斧→TreeObject、镐→RockObject。
  - 耐久/体力/水量在 `ToolInstance.Use` 中判定。

### 生理 PhysiologySystem
- `VitalContainer`/`VitalStat`（HP/Stamina/Satiety，`Reactive<float>` 驱动 UI）。
- `BuffModule` + `BuffFactory`：Instant/OverTime/Forever 三种 buff，吃食物(`EatFoodEvent`)从 `buff.json` 查表生效；`Start` 里注册一个永久掉饱食度 buff。

### 动物 AnimalSystem
- `ChickenCoop` + `Chicken`（`ChickenWander`/`ChickenConsume`/`ChickenGrowth`/`ChickenEggLayer` 组件）。
- 鸡长大后可下蛋(`Egg`)，鸡舍门口 `CoopBrand` 打开 `ChickenCoopUI` 放鸡，`CoopFoodBucket`/`CoopWaterBucket` 喂食/喂水。

### 怪物 MonsterSystem
- `EnemyBase` + `StateMachine`（Patrol/Chase/Attack/Hurt/Dead 五状态，`MonsterStateType` 枚举）。
- 视野/听觉探测玩家(`CanSeePlayer`)、`OnAttackHit` 范围伤害、死亡掉落上报任务。

### NPC NpcSystem
- 加载 `Npc.json` 生成 3 个 NPC（Mary/Rode/Vic）。
- `NpcController` F 交互 → `NPCInteractPanel`（剧情/聊天/送礼/离开）。

### 剧情 PlotSystem
- 数据流：`Plot.json` → `StoryData`（segments）→ `SegmentData`（nodes）→ `NodeData`（type+body，`NodeDataConverter` 自定义反序列化）→ `NodeFactory` 生成 `IStoryNode`。
- 节点类型：`PlayFilm`(视频)、`Dialogue`(打字机对话)、`GiveQuest`(发任务)、`Timeline`(空壳未实现)。
- `SegmentRunner` 协程逐节点顺序执行；任务由 `QuestModule` 统一登记/推进/结算，`QuestManager.ReportAction` 是各处上报入口。

### 生产 ProductionSystem
- `CookingModule` 读 `cooking.json` 配方；`CookingPot`(4 输入/1 输出/2 燃料) + `ProductionBuildingUI`。
- `ProductBuildingBase`：配方选择、燃料燃烧、进度、循环生产（Event 驱动 UI 刷新）。

### 载具 VehicleSystem
- `VehicleBase`（座位 + 上下车）、`VehicleController`（WASD 驾驶 + 速度/转向模型）、`VehicleSeat` 静态座位表、`PlayerVehicleController` 上/下车状态切换（`InputContext.Driving`）。

### 武器 WeaponSystem
- `WeaponSystem` 加载 `Prefab/Weapon` + `Weapon.json`，`ToolbarItemChangedEvent` 驱动装备/卸下。
- `RangedWeapon`（射线命中 + 对象池子弹 `BulletBase` 带重力）、`AmmoUI`、R 换弹（`CommandReduceInventoryAmount` 扣背包子弹）。

## 配置表（Resources/Data 生效）

| 文件 | 用途 | 状态 |
|---|---|---|
| `ItemData.json` | 物品表（id 1/3~9/21/22/32/33/51~54/100~103/200/201） | ✅ 已接线 |
| `tool.json` | 工具表（锄/镐/斧/镰/水壶/鱼竿） | ✅ |
| `PlantData.json` | 植物/种子表（南瓜/西红柿，`timePerStage`） | ✅ |
| `PrefabMapping.json` | 物品→掉落物预制体（当前仅 100→Rock） | ✅ |
| `Npc.json` | NPC 数据（含未用的对话/日程/礼物字段） | ✅ |
| `buff.json` | 食物 buff（水煮蛋→饱食+体力） | ✅ |
| `cooking.json` | 烹饪配方（水煮蛋/炖鸡汤） | ✅ |
| `Weapon.json` | 武器表（手枪） | ✅ |
| `Plot.json` | 剧情（5 个 segment，对话+发任务+影片） | ✅ |
| `Achievement/Mission/monster/quest_main/quest_side_1/setting.json` | 预留/设计稿 | ❌ 未接线 |
| `Plot1.json` | 旧剧情草稿 | ❌ 未接线 |

⚠️ `Assets/Data/` 与 `Assets/Resources/Data/` 是两份重复配置，**运行时只读 Resources 版**；其中 `PlantData/Npc/Plot` 两份内容不一致，易混淆，建议只保留一份。

## 操作键位

| 键 | 功能 |
|---|---|
| WASD / Shift / Space | 移动 / 冲刺 / 跳跃 |
| 鼠标 | 视角；左键=用工具/种植/攻击；右键=吃食物(快捷栏选中)/背包拆分 |
| F | 交互（拾取/对话/上车/开门） |
| 1~0 | 快捷栏切换 |
| B | 背包开关；P 商店；LeftAlt 显示/隐藏光标 |
| R | 换弹；Esc 关闭面板 |

## 已知问题 / 待办（已完成一轮修复）

> 2026-09 修复完成：3 处构建阻断（编辑器命名空间）、4 处空引用、以及暴击/喝水/生产扣除/退订/收获污染/倒计时/生理值越界等逻辑 bug 均已修复。以下为当前遗留项。

### 有意暂缓 / 待实现
- **货币系统未做**：`ShopItem.PurchaseItems` 直接 `TryAddItem`，不扣钱（用户暂不实现；`ItemData.price` 字段已就绪，接一个金币数值+扣钱即可）。
- **镰刀 / 鱼竿无行为**：`ToolBehaviorFactory` 对 `Sickle/FishingRod` 返回 `null`，`ToolModule` 已加判空拦截（不再崩溃，但装备后无法使用）。镰刀可照 `AxeBehaviour` 加「草地层」快速补齐；鱼竿是完整钓鱼玩法，工程量较大。
- **枚举 ↔ JSON 字符串强耦合**：`MusicType/NpcType/ItemSubType/SeedType/WeaponType` 等靠字符串精确匹配，新增需两端同步（用户接受现状）。

### 可选的设计 / 细节
- **收获后土地回到初始 Dry**（需重新锄地 + 浇水才能再种）。若想保持「已耕」状态，`FarmlandTile.Harvest` 里保留 `isPlowed` 即可。
- **成年鸡消耗未生效**：`ChickenGrowth.GrowUp` 改了 `hungerReduce/thirstReduce`（10/20），但 `ChickenConsume.UpdateConsume` 用的是硬编码 5/10，成年后「吃更多」没有体现。

## 命名 / 代码风格待清理（拼写错误）

- `PlayeMovement` → `PlayerMovement`；`SoliData` → `SoilData`；`IInteractalbe` → `IInteractable`（含 `Ineract` → `Interact`）；`CreatItemInWorld` → `CreateItemInWorld`；`VehicleSeat.Attack` → `Attach`、`Detack` → `Detach`；`CommandConsumeSatiely` → `CommandConsumeSatiety`；`ToolUseResolt` → `ToolUseResult`。
- 大部分源文件为 **GBK/GB2312 编码**（跨工具/Git 显示中文乱码），建议统一转 UTF-8。
- 大量冗余 `using Unity.VisualScripting;` / `System.Security.Cryptography` 等可清理。

## 运行 / 打包

- 用 Unity Hub 打开项目目录，主场景 `Assets/Scenes/SampleScene.unity`。
- 打包前确认无 `UnityEditor` / 第三方插件命名空间残留（编辑器命名空间引用已清理，若再引入插件注意检查）。
- 无自动化测试；`Test.cs`/`ReactiveTest.cs`/`TestCommand.cs` 为临时验证脚本，发布前应移除或挂到测试场景。
