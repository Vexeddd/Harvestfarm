using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionSystem : SingletonAutoMono<ProductionSystem>
{
    public CookingModule cookingModule;

    protected override void Awake()
    {
        base.Awake();

        cookingModule = UnityEngine.Object.FindObjectOfType<CookingModule>();
        if (cookingModule == null)
        {
            GameObject obj1 = new GameObject("CookingModule");
            cookingModule = obj1.AddComponent<CookingModule>();
            obj1.transform.parent = this.transform;
        }
    }
}
