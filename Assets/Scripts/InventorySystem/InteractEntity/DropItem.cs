using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : CollectItem
{
    private SpriteRenderer s_Renderer; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up, 180 * Time.deltaTime, Space.World); 
    }


    public override void SetData(ItemData itemData)
    {
        base.SetData(itemData);

        s_Renderer = GetComponent<SpriteRenderer>();
        if (s_Renderer != null)
        {
            s_Renderer.sprite = itemData.icon;
        }
    }
}
