using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSystem : SingletonAutoMono<AnimalSystem>
{
    public GameObject chicken_Prefab;
    public GameObject egg_prefab;
    protected override void Awake()
    {
        base.Awake();

        chicken_Prefab = Resources.Load<GameObject>("Prefab/Animal/Chicken");
        egg_prefab = Resources.Load<GameObject>("Prefab/Animal/Egg");
    }
}
