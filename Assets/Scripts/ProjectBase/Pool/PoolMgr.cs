using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PoolData
{
    public GameObject fatherObj;
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>();

        PushObj(obj);
    }

    public void PushObj(GameObject obj)
    {
        obj.SetActive(false);
        poolList.Add(obj);
        obj.transform.parent = fatherObj.transform;
    }

    public GameObject GetObj()
    {
        GameObject obj = null;
        obj = poolList[0];
        poolList.RemoveAt(0);

        obj.SetActive(true);
        obj.transform.parent = null;
        
        return obj;
    }
}

public class PoolMgr : BaseManager<PoolMgr>
{
    private PoolMgr()
    {

    }

    private GameObject poolObj;

    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();


    /// <summary>
    /// 从缓存池内拿
    /// </summary>
    /// <param name="name">资源名（路径）</param>
    /// <returns></returns>
    public void GetObj(string name, UnityAction<GameObject> callBack)
    {
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            //obj = poolDic[name].GetObj();
            callBack(poolDic[name].GetObj());
        }
        else
        {
            ResMgr.Instance.LoadAsync<GameObject>(name, (o) => 
            {
                o.name = name;
                callBack(o);
            });
            //obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            //obj.name = name;
        }
    }


    /// <summary>
    /// 存放到缓存池
    /// </summary>
    /// <param name="name">名字</param>
    /// <param name="obj">存放的物体</param>
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
        {
            poolObj = new GameObject("Pool");
        }

        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        else
        {
            poolDic.Add(name, new PoolData(obj, poolObj));
        }
    }

    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
