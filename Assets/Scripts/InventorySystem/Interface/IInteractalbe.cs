using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractalbe
{
    string GetInteractTip();

    bool CanInteract();

    void Ineract(GameObject obj);
}
