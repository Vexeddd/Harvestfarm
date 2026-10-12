using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesMgr : BaseManager<ScenesMgr>
{
    private ScenesMgr()
    {

    }
    public void LoadScene(string name, UnityAction action)
    {
        SceneManager.LoadScene(name);
        action();
    }

    public void LoadSceneAsync(string name, UnityAction action)
    {
        MonoMgr.Instance.StartCoroutine(LoadSceneAsyncCoroutine(name, action));
    }

    private IEnumerator LoadSceneAsyncCoroutine(string name, UnityAction action)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //yield return ao;
        //action();
        while (!ao.isDone)
        {
            EventCenter.Instance.EventTrigger("LoadingScene", ao.progress);
            yield return ao.progress;
        }
        action();
    }
}
