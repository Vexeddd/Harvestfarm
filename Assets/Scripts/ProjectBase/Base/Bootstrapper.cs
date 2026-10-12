using UnityEngine;

public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitAll()
    {
        // 在这里按需触发你希望提前创建的单例


        // var _3 = UIMgr2.Instance;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitAllThen()
    {
        // 在这里按需触发你希望提前创建的单例
        var _1 = UIMgr.Instance;
        var _2 = LoadManager.Instance;
        var _3 = InventorySystem.Instance;
        var _4 = AgricultureSystem.Instance;
        var _5 = AnimalSystem.Instance;
        var _6 = PhysiologySystem.Instance;
        var _7 = ProductionSystem.Instance;
        var _8 = WeaponSystem.Instance;
        var _9 = MonsterSystem.Instance;
        var _10 = NpcSystem.Instance;
        var _11 = PlotSystem.Instance;
        var _12 = VehicleSystem.Instance;
        var _13 = QuestManager.Instance;
        var _14 = MusicManager.Instance;
    }
}