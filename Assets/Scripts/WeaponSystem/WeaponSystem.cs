using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : SingletonAutoMono<WeaponSystem>
{
    public WeaponBase currentWeapon;

    public Dictionary<WeaponType, GameObject> weapon_prefab_dict = new Dictionary<WeaponType, GameObject>();
    public Dictionary<int, WeaponData> weapon_data_dict = new Dictionary<int, WeaponData>();

    public Transform weapon_mount_point;

    public GameObject bullet_9mm_Prefab;
    public string bullet_9mm_PoolKey = "Prefab/Bullet/Bullet_9mm";

    protected override void Awake()
    {
        base.Awake();

        GameObject[] weapons = Resources.LoadAll<GameObject>("Prefab/Weapon");
        foreach (GameObject weapon in weapons)
        {
            WeaponType type = Enum.Parse<WeaponType>(weapon.name);
            weapon_prefab_dict.Add(type, weapon);
        }

        weapon_mount_point = GameObject.Find("weapon_mount_point").transform;

        TextAsset text = Resources.Load<TextAsset>("Data/Weapon");
        List<WeaponData> weaponDatas = JsonConvert.DeserializeObject<List<WeaponData>>(text.text);
        foreach (var data in weaponDatas)
        {
            weapon_data_dict.Add(data.item_id, data);
        }

        bullet_9mm_Prefab = Resources.Load<GameObject>("Prefab/Bullet/Bullet_9mm");
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ToolbarItemChangedEvent>(OnToolbarItemChanged);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ToolbarItemChangedEvent>(OnToolbarItemChanged);
    }


    private void OnToolbarItemChanged(ToolbarItemChangedEvent @event)
    {
        InventoryItem item = @event.inventoryItem;
        //ItemData weaponData = @event.inventoryItem.itemData;
        //如果切换的槽位为空 或者不为武器
        if (item == null || item.itemData == null || item.itemData.item_type != ItemType.Weapon)
        {
            //如果之前手上有武器
            if (currentWeapon != null)
            {
                //卸下
                UnEquip();
            }
            return;
        }

        //如果切换的槽位是武器类

        //如果之前有武器
        if (currentWeapon != null)
        {
            //卸下
            UnEquip();
        }
        
        //装备新武器
        Equip(item.itemData);
    }

    private void Equip(ItemData itemData)
    {
        WeaponType weaponType = Enum.Parse<WeaponType>(itemData.item_sub_type.ToString());
        GameObject weapon_Prefab = weapon_prefab_dict[weaponType];
        GameObject weapon = GameObject.Instantiate(weapon_Prefab, weapon_mount_point);

        WeaponBase weaponScp = weapon.GetComponent<WeaponBase>();

        //给武器注入数据，初始化
        WeaponData weaponData = weapon_data_dict[itemData.id];

        WeaponRuntimeState weaponState;
        if (StateManager.GetState<WeaponRuntimeState>(itemData.uuid) is null)
        {
            weaponState = new WeaponRuntimeState(itemData.uuid);
            //toolState.current_duration = toolData.max_durability;   //武器不做耐久系统

            StateManager.RegisterState(itemData.uuid, weaponState);
        }
        else
        {
            weaponState = StateManager.GetState<WeaponRuntimeState>(itemData.uuid);
        }

        weaponScp.Init(weaponData, weaponState);
        currentWeapon = weaponScp;

        //装备动画..声音..



        //针对远程武器 显示子弹ui
        if (currentWeapon.weaponData.ballistic_type == BallisticType.Ranged)
        {
            AmmoUI.Instance.OpenContainer();

            (currentWeapon as RangedWeapon).currentAmmo = weaponState.current_ammo;

            int bag_amount = 0;
            EventBus.Publish(new ItemQueryRequestEvent
            {
                item_id = weaponData.bullet_id,
                callBack = (val) =>
                {
                    bag_amount = val;
                }
            });

            AmmoUI.Instance.SetCurrentAmmo((currentWeapon as RangedWeapon).currentAmmo);
            AmmoUI.Instance.SetBagAmmo(bag_amount);
        }

    }

    private void UnEquip()
    {
        if (currentWeapon.weaponData.ballistic_type == BallisticType.Ranged)
        {
            currentWeapon.weaponRuntimeState.current_ammo = (currentWeapon as RangedWeapon).currentAmmo;
            AmmoUI.Instance.CloseContainer();
        }
        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
    }


    private void Update()
    {
        if (currentWeapon != null && Input.GetMouseButtonDown(0))
        {
            currentWeapon.Attack();
        }
    }

    private void OnDrawGizmos()
    {

        if (currentWeapon == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0)));




    }
}
