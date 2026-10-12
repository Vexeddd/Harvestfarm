using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BaseManager<T> where T : class //,new()
{
    protected static readonly object _lock = new object();
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        //_instance = new T();
                        Type type = typeof(T);
                        ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                                                                              null,
                                                                              Type.EmptyTypes,
                                                                              null);
                        if (constructorInfo != null)
                        {
                            _instance = constructorInfo.Invoke(null) as T;
                        }
                        else
                        {
                            Debug.LogError("没有得到相应构造函数");
                        }
                    }
                }
            }
            return _instance;
        }
    }
}   
