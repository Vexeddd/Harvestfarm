using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputMgr : BaseManager<InputMgr>
{
    private bool isInputChecked;

    private InputMgr()
    {
        MonoMgr.Instance.AddUpdateListener(InputUpdate);
    }

    public void IsInputChecked(bool isChecked)
    {
        isInputChecked = isChecked;
    }

    private void InputUpdate()
    {
        if (!isInputChecked)
            return;

        KeyboardDetection(KeyCode.W);
        KeyboardDetection(KeyCode.S);
        KeyboardDetection(KeyCode.A);
        KeyboardDetection(KeyCode.D);

    }

    private void KeyboardDetection(KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode))
        {
            EventCenter.Instance.EventTrigger("KeyDown", keyCode);
        }
        if (Input.GetKeyUp(keyCode))
        {
            EventCenter.Instance.EventTrigger("KeyUp", keyCode);
        }
    }
}
