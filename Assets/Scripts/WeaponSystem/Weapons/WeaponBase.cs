using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public WeaponData weaponData;
    public WeaponRuntimeState weaponRuntimeState;

    public bool IsAiming;

    public float lastAttackTime;

    public bool isDialog = false;

    public virtual void Init(WeaponData weaponData, WeaponRuntimeState weaponRuntime)
    {
        this.weaponData = weaponData;
        this.weaponRuntimeState = weaponRuntime;
    }

    //是否可以攻击
    public bool CanAttack()
    {
        //记录上次开枪的时间,  1.20  
        //间隔是1秒 2.20
        //当前时间 >= 记录上次开枪的时间 + 开枪间隔
        return (Time.time >= lastAttackTime + weaponData.attack_cooldown) && !isDialog;

    }

    //计算伤害
    public float CalDamage()
    {
        float dmg = weaponData.damage;

        //5% 100枪有5发暴击  0.05
        // 0, 0.01 0.02 0.03 0.04 .......   0.10 ..0.11....0.98 0.99 1.00
        if (Random.value <= weaponData.crit_rate)
        {
            dmg *= weaponData.crit_multiplier;
        }

        return dmg;
    }

    public void SetAim(bool isAiming)
    {
        this.IsAiming = isAiming;
    }

    public abstract void Attack();



    public virtual void Update()
    {

    }
}
