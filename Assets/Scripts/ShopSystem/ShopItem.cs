using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData itemData;

    public Button shopitemBtn;
    public Image shopitemImage;
    public TMP_Text shopitemPrice; 

    private void Awake()
    {
        shopitemBtn = GetComponent<Button>();

        shopitemImage = transform.Find("ShopItemIcon").GetComponent<Image>();
        shopitemPrice = transform.Find("ShopItemPrice").GetComponent<TMP_Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        shopitemBtn.onClick.AddListener(() =>
        {
            PurchaseItems();
        });
    }

    private void PurchaseItems()
    {
        QuestManager.Instance.ReportAction(QuestType.BuyItem, itemData.id.ToString(), 1);
        InventorySystem.Instance.TryAddItem(itemData, 1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null)
        {
            InventorySystem.Instance.tip.SetUI(itemData);
            InventorySystem.Instance.tip.IsOpen = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventorySystem.Instance.tip.SetUI(null);
        InventorySystem.Instance.tip.IsOpen = false;
    }


    public void SetData(ItemData itemData)
    {
        this.itemData = itemData;

        shopitemImage.sprite = itemData.icon;
        shopitemPrice.text = itemData.price.ToString() + "<sprite=9>"; 
    }
}
