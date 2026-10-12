using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCommand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestDecHp());
    }

    IEnumerator TestDecHp()
    {
        int a = 0;
        yield return new WaitForSeconds(5);
        while (a < 5)
        {
            print("¿ÛÑªÒ»´Î");
            a ++;
            CommandBus.Send(new CommandConsumeHP
            {
                amount = 10
            });
            yield return new WaitForSeconds(2);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StopCoroutine(TestDecHp());
        }
    }
}
