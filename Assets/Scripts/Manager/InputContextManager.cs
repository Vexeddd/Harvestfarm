using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputContextManager : SingletonAutoMono<InputContextManager>
{
    protected override string RootNodeName => "[Manager]";

    public InputContext Current { get; private set; } = InputContext.GamePlay;

    public void SetContext(InputContext context)
    {
        if (Current == context)
        {
            return;
        }

        Current = context;
    }
}

public enum InputContext
{
    GamePlay,    //玩家控制
    Driving      //载具控制
}