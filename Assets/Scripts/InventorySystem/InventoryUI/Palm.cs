using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Palm : InventoryContainer
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (slotList[0].bindItem != null)
        {
                FollowMouse();
        }
    }

    public override List<InventorySlot> InitContainer(int slotCount)
    {
        var list = base.InitContainer(slotCount);
        list[0].GetComponent<Image>().enabled = false;

        return list;
    }


    private void FollowMouse()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(UIMgr.Instance.canvas.GetComponent<RectTransform>(),
                                                                Input.mousePosition, 
                                                                UIMgr.Instance.UICamera,
                                                                out Vector2 localPoint);


        //(slotList[0].bindItem.transform as RectTransform).anchoredPosition = Vector2.Lerp((slotList[0].bindItem.transform as RectTransform).anchoredPosition,
        //                                                                                   localPoint,
        //                                                                                   Time.deltaTime * 35);
        (slotList[0].bindItem.transform as RectTransform).anchoredPosition = localPoint;

        //(slotList[0].bindItem.transform as RectTransform).anchoredPosition = localPoint;
    }

    public void PickUpItem(InventorySlot slot)
    {
        slotList[0].PickUpItem(slot.bindItem);

        slot.bindItem = null;
    }
}
