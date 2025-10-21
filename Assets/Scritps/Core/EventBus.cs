using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, Delegate> map = new();

    public static void Subscribe<T>(Action<T> cb)
    {
        if (map.TryGetValue(typeof(T), out var d)) map[typeof(T)] = Delegate.Combine(d, cb);
        else map[typeof(T)] = cb;
    }

    public static void Unsubscribe<T>(Action<T> cb)
    {
        if (map.TryGetValue(typeof(T), out var d)) map[typeof(T)] = Delegate.Remove(d, cb);
    }

    public static void Publish<T>(T evt)
    {
        if (map.TryGetValue(typeof(T), out var d)) (d as Action<T>)?.Invoke(evt);
    }
}
