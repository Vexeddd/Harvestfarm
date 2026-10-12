using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : BaseManager<MonoMgr>
{
    private MonoController controller;

    private MonoMgr()
    {
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<MonoController>();
    }

    public void AddUpdateListener(UnityAction action)
    {
        controller.AddUpdateListener(action);
    }

    public void RemoveUpdateListener(UnityAction action)
    {
        controller?.RemoveUpdateListener(action);
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return controller.StartCoroutine(routine);
    }

    public void StopCoroutine(IEnumerator routine)
    {
        controller.StopCoroutine(routine);
    }
}
