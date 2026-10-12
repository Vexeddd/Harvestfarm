using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    protected virtual string RootNodeName => "[System]";

    private static Transform _systemRoot;
    private static T _instance;

    private static readonly object _lock = new object();
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
                        // 优先在场景中查找（支持编辑器预挂载）
                        _instance = Object.FindObjectOfType<T>();
                        if (_instance != null) return _instance;

                        GameObject monoObj = new GameObject(typeof(T).Name);

                        //if (_systemRoot == null)
                        //{
                        //    GameObject systemGo = GameObject.Find("[System]");
                        //    if (systemGo == null)
                        //    {
                        //        systemGo = new GameObject("[System]");
                        //        Object.DontDestroyOnLoad(systemGo);
                        //    }
                        //    _systemRoot = systemGo.transform;
                        //}
                        //monoObj.transform.SetParent(_systemRoot, false);

                        // 添加组件并存储引用
                        _instance = monoObj.AddComponent<T>();
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            Transform root = GetOrCreateRoot(RootNodeName);
            transform.SetParent(root, false);
                
            if (root != null)
                DontDestroyOnLoad(root.gameObject);
        }
        else if (_instance != this)
        {
            // 已存在单例，销毁重复的实例（注意：如果你想在编辑器保留其 GameObject，可改为 Destroy(this)）
            Destroy(this.gameObject);
        }
    }

    private static Transform GetOrCreateRoot(string rootName)
    {
        GameObject rootGo = GameObject.Find(rootName);
        if (rootGo == null)
        {
            rootGo = new GameObject(rootName);
            Object.DontDestroyOnLoad(rootGo);
        }
        return rootGo.transform;
    }
}
 