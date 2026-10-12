using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 事件中心（新版）
/// </summary>
public static class EventBus
{
    private readonly static Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(Action<T> callback)
    {
        if (!eventTable.ContainsKey(typeof(T)))
        {
            eventTable[typeof(T)] = null;
        }

        eventTable[typeof(T)] = (Action<T>)eventTable[typeof(T)] + callback;
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (eventTable.TryGetValue(typeof(T), out var del))
        {
            eventTable[typeof(T)] = (Action<T>)del - callback;
        }
    }

    public static void Publish<T>(T eventData)
    {
        if (eventTable.TryGetValue(typeof(T), out var del))
        {
            (del as Action<T>)?.Invoke(eventData);
        }
    }


    public static void Clear()
    {
        eventTable.Clear();
    }
}