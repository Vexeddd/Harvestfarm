using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenEggLayer : MonoBehaviour
{
    public Chicken chicken;

    public float minLayDuration = 3f;
    public float maxLayDuration = 10f;

    public float nextLayTime;
    public float timer;

    void Start()
    {
        GetRandomEggTime();
    }


    void Update()
    {
        if (!chicken.isAdult)
        {
            return;
        }

        timer += Time.deltaTime;
        if (timer >= nextLayTime)
        {
            LayEgg();
            GetRandomEggTime();
            timer = 0;
        }
    }


    private void GetRandomEggTime()
    {
        nextLayTime = UnityEngine.Random.Range(minLayDuration, maxLayDuration);
    }

    public void LayEgg()
    {
        GameObject eggPrefab = AnimalSystem.Instance.egg_prefab;

        GameObject.Instantiate(eggPrefab, this.transform.position, Quaternion.identity);
    }
}
