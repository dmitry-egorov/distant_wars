using UnityEngine;
using UnityEngine.Profiling;

public static class Massive
{
    public static void _<T>() where T : class, MassiveMechanic, new() => run<T>();
    public static void run<T>() where T: class, MassiveMechanic, new()
    {
        Profiler.BeginSample(Registry<T>.Name);
        try
        {
            Registry<T>.Instance._();
        }
        finally
        {
            Profiler.EndSample();
        }
    }

    static class Registry<T> where T : class, new()
    {
        public static string Name => name ?? (name = typeof(T).Name);
        static string name;
        
        public static T Instance => instance ?? (instance = new T());
        static T instance;
    }
}