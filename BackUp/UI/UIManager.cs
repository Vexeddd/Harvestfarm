using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System
}

public class UIManager : BaseManager<UIManager>
{
    private Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    public RectTransform canvas;

    private UIManager()
    {
        GameObject obj = ResMgr.Instance.Load<GameObject>("UIBase/Canvas");
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj);

        bot = canvas.Find("bot");
        mid = canvas.Find("mid");
        top = canvas.Find("top");
        system = canvas.Find("system");


        obj = ResMgr.Instance.Load<GameObject>("UIBase/EventSystem");
        GameObject.DontDestroyOnLoad(obj);

    }

    public Transform GerLayerFather(E_UI_Layer layer)
    {
        switch (layer)
        {
            case E_UI_Layer.Bot:
                return this.bot;
            case E_UI_Layer.Mid:
                return this.mid;
            case E_UI_Layer.Top:
                return this.top;
            case E_UI_Layer.System:
                return this.system;
        }
        return null;
    }

    public void ShowPanel<T>(string panelName, E_UI_Layer layer, UnityAction<T> callback = null) where T : BasePanel
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowMe();
            if (callback != null)
            {
                callback(panelDic[panelName] as T);
                return;
            }
        }

        ResMgr.Instance.LoadAsync<GameObject>("UIBase/" + panelName, (obj) =>
        {
            Transform father = bot;
            switch (layer)
            {
                case E_UI_Layer.Mid:
                    father = mid;
                    break;
                case E_UI_Layer.Top:
                    father = top;
                    break;
                case E_UI_Layer.System:
                    father = system;
                    break;
            }

            obj.transform.SetParent(father, false);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            T panel = obj.GetComponent<T>();
            if (callback != null)
            {
                callback(panel);
            }

            panel.ShowMe();

            panelDic.Add(panelName, panel);

        });
    }

    public void HidePanel(string panelName)
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].HideMe();
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }

    public T GetPanel<T>(string panelName) where T : BasePanel
    {
        if (panelDic.ContainsKey(panelName))
        {
            return panelDic[panelName] as T;
        }
        return null;
    }

    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = control.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}
