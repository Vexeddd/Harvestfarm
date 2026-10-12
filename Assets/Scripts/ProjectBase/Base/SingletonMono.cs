using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get { return _instance; }
    }
    protected virtual void Awake()
    {
        _instance = this as T;
    }

}
