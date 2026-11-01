using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;


public interface IEventAction
{

}

public class EventAction : IEventAction
{
    public UnityAction actions;

    public EventAction(UnityAction action)
    {
        actions = action;
    }
}

public class EventAction<T> : IEventAction
{
    public UnityAction<T> actions;

    public EventAction(UnityAction<T> action)
    {
        actions = action;
    }
}

public class EventCenter : BaseManager<EventCenter>
{
    private EventCenter()
    {

    }

    private Dictionary<string, IEventAction> eventDic = new Dictionary<string, IEventAction>();


    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name] += action;
            (eventDic[name] as EventAction).actions += action;
        }
        else
        {
            //eventDic.Add(name, action);
            eventDic.Add(name, new EventAction(action));
        }
    }

    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name] += action;
            (eventDic[name] as EventAction<T>).actions += action;
        }
        else
        {
            //eventDic.Add(name, action);
            eventDic.Add(name, new EventAction<T>(action));
        }
    }


    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name] -= action;
            (eventDic[name] as EventAction).actions -= action;
        }
    }

    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name] -= action;
            (eventDic[name] as EventAction<T>).actions -= action;
        }
    }


    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name].Invoke(info); 
            (eventDic[name] as EventAction).actions.Invoke();
        }
    }

    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            //eventDic[name].Invoke(info); 
            (eventDic[name] as EventAction<T>).actions?.Invoke(info);
        }
    }

    public void Clear()
    {
        eventDic.Clear();
    }
}
