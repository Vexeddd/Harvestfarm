using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageboxEntity : MonoBehaviour, IInteractalbe
{
    public List<ItemData> itemDatas = new List<ItemData>();
    public bool isInteracting = false;

    private void Update()
    {
        if (!isInteracting)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F))
        {
            itemDatas = InventorySystem.Instance.storagebox.CloseBox();
            SetInteractable(true);
        }
    }

    public string GetInteractTip()
    {
        return $"按F打开储物箱 {this.name}";
    }

    public bool CanInteract()
    {
        return !isInteracting;
    }


    public void Ineract(GameObject obj)
    {
        OpenBox();
    }

    public void SetInteractable(bool can)
    {
        isInteracting = !can;
    }

    private void OpenBox()
    {
        InventorySystem.Instance.storagebox.OpenBox(itemDatas);
        SetInteractable(false);
    }
}
