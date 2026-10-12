using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoUI : UIContainer
{
    public TMP_Text currentAmmo;
    public TMP_Text maxAmmo;


    private static AmmoUI _instance;
    public static AmmoUI Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        _instance = this as AmmoUI;

        currentAmmo = transform.FindRecursive("CurrentAmmo").GetComponent<TMP_Text>();
        maxAmmo = transform.FindRecursive("MaxAmmo").GetComponent<TMP_Text>();
    }


    public int GetBagAmmoCount(int bullet_id)
    {
        int amount = 0;
        EventBus.Publish(new ItemQueryRequestEvent
        {
            item_id = bullet_id,
            callBack = (val) =>
            {
                amount = val;
            }
        });

        return amount;
    }

    public void SetCurrentAmmo(int count)
    {
        currentAmmo.text = count.ToString();
    }

    public void SetBagAmmo(int count)
    {
        maxAmmo.text = count.ToString(); 
    }
}
