using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : SingletonAutoMono<UIMgr>
{
    protected override string RootNodeName => "[Manager]";

    public Camera UICamera;
    public Canvas canvas;

    public GameObject crossHair;

    protected override void Awake()
    {
        base.Awake();
        UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        crossHair = GameObject.Find("CrossHair");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            ShowCursor();
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            HideCursor();
        }
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
