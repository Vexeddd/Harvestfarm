using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResMgr : BaseManager<ResMgr>
{
    private ResMgr()
    {

    }

    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if (res == null)
        {
            Debug.LogError($"Resource 不存在：{name}");
            return null;
        }

        if (res is GameObject)
        {
            return GameObject.Instantiate(res) as T;
        }
        else
        {
            return res;
        }
    }

    public void LoadAsync<T>(string name, UnityAction<T> callBack) where T : Object
    {
        if (callBack == null)
        {
            Debug.LogWarning("LoadAsync 回调为空，已忽略请求。");
            return;
        }

        MonoMgr.Instance.StartCoroutine(LoadAsyncCoroutine<T>(name, callBack));
    }

    private IEnumerator LoadAsyncCoroutine<T>(string name, UnityAction<T> callBack) where T : Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(name);
        yield return request;

        if (request.asset == null)
        {
            Debug.LogError($"资源加载失败：{name}");
            yield break;
        }


        if (request.asset is GameObject)
        {
            callBack?.Invoke(GameObject.Instantiate(request.asset) as T);
        }
        else
        {
            callBack?.Invoke(request.asset as T);
        }
    }
}
