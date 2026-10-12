using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour, IInteractalbe
{
    [SerializeField]
    public ItemData itemData;


    public void Collect()
    {
        InventorySystem.Instance.TryAddItem(itemData, itemData.current_stack);

        //上报任务
        QuestManager.Instance.ReportAction(QuestType.CollectItem, itemData.id.ToString(), itemData.current_stack);

        Destroy(gameObject);
    }

    public virtual void SetData(ItemData itemData)
    {
        this.itemData = itemData;
    }

    public string GetInteractTip()
    {
        return $"按F 拾取 {itemData.name}";
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Ineract(GameObject obj)
    {
        Collect();
    }
}
