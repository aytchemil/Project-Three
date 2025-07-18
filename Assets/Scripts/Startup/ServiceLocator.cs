using System;
using System.Collections.Generic;

public static class ServiceLocator
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<T>(T service)
    {
        var type = typeof(T);
        if (!services.ContainsKey(type))
        {
            services.Add(type, service);
        }
        else
        {
            services[type] = service; // overwrite if already exists
        }
    }

    public static T Get<T>()
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out var service))
        {
            return (T)service;
        }
        throw new Exception($"Service of type {type} is not registered!");
    }

    public static bool Exists<T>()
    {
        return services.ContainsKey(typeof(T));
    }

    public static void Clear()
    {
        services.Clear();
    }
}