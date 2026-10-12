using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactive<T>
{
    private T _value;

    public event Action<T> onValueChanged;

    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
            {
                return;
            }

            _value = value;
            onValueChanged?.Invoke(_value);
        }
    }

    //构造函数
    public Reactive(T defaultValue = default)
    {
        _value = defaultValue;
    }


    //订阅事件
    public void Subscribe(Action<T> callBack, bool callImmediately = false)
    {
        onValueChanged += callBack;

        if (callImmediately)
        {
            callBack(_value);
        }
    }
}
