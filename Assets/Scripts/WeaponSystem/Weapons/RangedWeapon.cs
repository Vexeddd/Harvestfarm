using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[SerializeField]
public class RangedWeapon : WeaponBase
{
    public int currentAmmo; //当前弹夹子弹

    public bool isReloading = false;

    public Transform _firePoint;

    public override void Init(WeaponData weaponData, WeaponRuntimeState weaponRuntime)
    {
        base.Init(weaponData, weaponRuntime);
        _firePoint = transform.FindRecursive("FirePoint");
        if (_firePoint == null)
        {
            _firePoint = this.transform;
        }
    }

    public override void Attack()
    {
        //1.判断子弹攻击间隔 或者  在换弹
        if (!CanAttack() || isReloading)
        {
            return;
        }

        //2.判断弹夹是否为空
        if (weaponRuntimeState.current_ammo <= 0)
        {
            //播放音效
            MusicManager.Instance.PlayOneShot(MusicType.EmptyBullet);

            return;
        }

        //3.生成子弹
        //Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        //GameObject bulletObj = Instantiate(WeaponSystem.Instance.bullet_9mm_Prefab, _firePoint);
        //bulletObj.GetComponent<BulletBase>().Init(ray.direction, weaponData, CalDamage());

        Ray camRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        Vector3 targetPoint;
        if (Physics.Raycast(camRay, out RaycastHit hit, 500f, ~0, QueryTriggerInteraction.Ignore))
        {
            targetPoint = hit.point;
        }
        else
        {
            // 没打到任何东西，就取相机前方 500 米处作为目标点
            targetPoint = camRay.GetPoint(500f);
        }
        Vector3 fireDir = (targetPoint - _firePoint.position).normalized;

        PoolMgr.Instance.GetObj(WeaponSystem.Instance.bullet_9mm_PoolKey, (bulletObj) =>
        {
            bulletObj.transform.position = _firePoint.position;
            bulletObj.transform.rotation = Quaternion.LookRotation(fireDir);

            var bullet = bulletObj.GetComponent<BulletBase>();
            bullet.Init(fireDir, weaponData, CalDamage(), WeaponSystem.Instance.bullet_9mm_PoolKey);
        });

        //播放音效
        MusicManager.Instance.PlayOneShot(MusicType.Shoot);


        //4.扣减子弹
        currentAmmo -= 1;
        weaponRuntimeState.current_ammo -= 1;
        AmmoUI.Instance.SetCurrentAmmo(weaponRuntimeState.current_ammo);

        //5.发射射线检测
        //移到子弹脚本内
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("换弹");
            CheckReload();
        }
    }

    //是否能装弹 ： 弹夹是否满，背包是否有足够子弹
    public void CheckReload()
    {
        int bag_Ammo = AmmoUI.Instance.GetBagAmmoCount(weaponData.bullet_id);

        if (currentAmmo == weaponData.ammo_size || bag_Ammo == 0)
        {
            return;
        }

        int requirement = weaponData.ammo_size - currentAmmo;
        int consume = Mathf.Min(requirement, bag_Ammo);

        //扣减子弹
        CommandBus.Send(new CommandReduceInventoryAmount
        {
            item_id = weaponData.bullet_id,
            amount = consume,
        });

        //更新右下角子弹UI
        bag_Ammo -= consume;
        AmmoUI.Instance.OpenContainer();
        AmmoUI.Instance.SetBagAmmo(bag_Ammo);

        //装弹

        //播放音效
        MusicManager.Instance.PlayOneShot(MusicType.Reload);

        isReloading = true;
        StartCoroutine(Reload(consume));
    }


    IEnumerator Reload(int consume)
    {
        yield return new WaitForSeconds(weaponData.reload_time);

        isReloading = false;
        currentAmmo += consume;
        weaponRuntimeState.current_ammo = currentAmmo;

        //更新子弹UI
        AmmoUI.Instance.SetCurrentAmmo(currentAmmo);
        //查询更新背包子弹UI，以防换弹过程中获得子弹
        AmmoUI.Instance.SetBagAmmo(AmmoUI.Instance.GetBagAmmoCount(weaponData.bullet_id));
    }
}
