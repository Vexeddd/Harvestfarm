using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveTest : MonoBehaviour
{
    public Reactive<int> HP = new Reactive<int>(10);

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        HP.Subscribe(DecHp, true);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 2f)
        {
            timer = 0;
            HP.Value -= 1;
        }
    }

    public void DecHp(int hp)
    {
        Debug.Log("—™¡øºı…Ÿ¡À" + hp); 
    }
}
