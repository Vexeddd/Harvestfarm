using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Trashcan : UIContainer
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!IsOpen)
            {
                IsOpen = true;
            }
            else
            {
                IsOpen = false;
            }
        }
    }
}
