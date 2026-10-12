using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour, IInteractalbe
{
    public bool CanInteract()
    {
        return true;
    }

    public string GetInteractTip()
    {
        return "°´F Ê°È¡¼¦µ°";
    }

    public void Ineract(GameObject obj)
    {
        InventorySystem.Instance.TryAddItem(53);

        Destroy(this.gameObject);
    }
}
