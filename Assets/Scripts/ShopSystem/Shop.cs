using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : UIContainer
{
    public ScrollRect shopScrollView;

    private List<ItemData> items;   

    private void Awake()
    {
        shopScrollView = this.transform.Find("ShopScrollView").GetComponent<ScrollRect>();
    }
    // Start is called before the first frame update
    void Start()
    {
        items = LoadManager.Instance.itemDatas;

        foreach (var item in items)
        {
            GameObject shopitemgo =  GameObject.Instantiate(InventorySystem.Instance.shopitemPrefab, shopScrollView.content);
            shopitemgo.GetComponent<ShopItem>().SetData(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!IsOpen)
            {
                IsOpen= true;
            }
            else
            {
                IsOpen = false;
            }
        }
    }
}
