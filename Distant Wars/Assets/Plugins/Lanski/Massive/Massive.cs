using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public static class Massive
{
    public static void _<T>()   where T : class, MassiveMechanic, new() => run<T>();
    public static void run<T>() where T : class, MassiveMechanic, new()
    {
        sw.Restart();
        var name = Registry<T>.Name;
        Profiler.BeginSample(name);
        try
        {
            Registry<T>.Instance._();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            MassiveDebug.Display(e.ToString());
        }
        finally
        {
            var ticks = sw.ElapsedTicks;
            total_times[name] = total_times.ContainsKey(name) ? total_times[name] +  ticks : ticks;
            Profiler.EndSample();
        }
    }

    public static void finish_frame()
    {
        total_frames++;
    }

    public static (string name, string time) show_time_for<T>() where T: class, MassiveMechanic, new()
    {
        var name = Registry<T>.Name;
        var text = (Math.Floor((total_times[name] / (Stopwatch.Frequency / 1_000_000L)) / (double)total_frames) / 1000.0).ToString() + "ms";

        return (name, text);
    }

    static class Registry<T> where T : class, new()
    {
        public static string Name => name ?? (name = typeof(T).Name);
        static string name;
        
        public static T Instance => instance ?? (instance = new T());
        static T instance;
    }

    static Stopwatch sw = new Stopwatch();
    static uint total_frames;
    static Dictionary<string, long> total_times = new Dictionary<string, long>();
}