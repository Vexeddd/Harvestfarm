using System;

[Serializable]
public class WeaponData
{
    public string id; //id
    public int item_id; //对应的物品id
    public string name; //名称

    public WeaponType weapon_type; //武器类型

    //子弹相关
    public BulletType bullet_type; //子弹类型
    public int bullet_id; //使用的子弹id
    public int ammo_size; //弹夹容量

    //开火相关
    public FireMode fire_mode; //开火模式
    public BallisticType ballistic_type; //弹道模式
    public bool can_switch_fireMode; //是否可以切换开火模式

    //武器参数相关
    public float damage; //伤害
    public float crit_rate; //暴击率
    public float crit_multiplier; //暴击倍数
    public float arrow_speed; //子弹速度
    public float attack_cooldown; //最小射击间隔
    public float reload_time; //装弹时间
    public float ads_zoom; //默认瞄准镜放大倍率
    public float ads_move_speed; //开启瞄准后的移速

}

/// <summary>
/// 子弹类型
/// </summary>
public enum BulletType
{
    Bullet_9mm, //手枪 冲锋枪
}

/// <summary>
/// 开火模式
/// </summary>
public enum FireMode
{
    Semi, //单发
    Auto, //全自动
}

/// <summary>
/// 武器弹道类型
/// </summary>
public enum BallisticType
{
    Melee, //近战
    Ranged, //远程
    Throw, //投掷物
    Place, //放置物
}