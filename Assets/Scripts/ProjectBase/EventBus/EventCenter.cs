using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;


public interface IEventInfo
{

}

public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions = action;
    }
}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions = action;
    }
}

public class EventCenter : BaseManager<EventCenter>
{
    private EventCenter()
    {

    }

    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();


    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name] += action;
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            //eventDic.Add(name, action);
            eventDic.Add(name, new EventInfo(action));
        }
    }

    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name] += action;
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            //eventDic.Add(name, action);
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }


    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name] -= action;
            (eventDic[name] as EventInfo).actions -= action;
        }
    }

    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name] -= action;
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }


    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name].Invoke(info); 
            (eventDic[name] as EventInfo).actions.Invoke();
        }
    }

    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name].Invoke(info); 
            (eventDic[name] as EventInfo<T>).actions?.Invoke(info);
        }
    }

    public void Clear()
    {
        eventDic.Clear();
    }
}
