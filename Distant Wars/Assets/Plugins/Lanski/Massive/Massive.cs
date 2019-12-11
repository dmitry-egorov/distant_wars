using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public static class Massive
{
    public static void _<T>()   where T : class, MassiveMechanic, new() => run<T>();
    public static void run<T>() where T : class, MassiveMechanic, new()
    {
        mechanics_sw.Restart();
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
            var ticks = mechanics_sw.ElapsedTicks;
            var old_time = average_frame_times.ContainsKey(name) ? average_frame_times[name] : ticks;

            var w = moving_average_weight;
            average_frame_times[name] = old_time * (1.0 - w) + w * ticks;
            average_frame_times_updated[name] = true;
            Profiler.EndSample();
        }
    }

    public static void start_update()
    {
        update_sw.Restart();
    }

    public static void finish_update()
    {
        total_frames++;
        var w = moving_average_weight;
        average_frame_time = (1.0 - w) * average_frame_time + w * frame_sw.ElapsedTicks;
        frame_sw.Restart();

        average_update_time = (1.0 - w) * average_update_time + w * update_sw.ElapsedTicks;

        var keys = average_frame_times_updated.Keys.ToArray();
        foreach (var key in keys)
        {
            if (!average_frame_times_updated[key])
            {
                average_frame_times[key] = average_frame_times[key] * (1.0 - w);
            }
            else
            {
                average_frame_times_updated[key] = false;
            }
        }
    }

    public static string get_avg_frame_time() => to_ms_string(average_frame_time);
    public static string get_avg_update_time() => to_ms_string(average_update_time);

    public static IEnumerable<(string name, string time)> get_top_avg_times(int count) => 
        average_frame_times
            .OrderByDescending(x => x.Value)
            .Take(count)
            //.OrderByDescending(x => Math.Floor(3.0 * x.Value / (Stopwatch.Frequency / 1_000)))
            //.ThenBy(x => x.Key)
            .Select(x => (x.Key, to_ms_string(x.Value)))
        ;

    public static (string name, string time) get_avg_time_for<T>() where T: class, MassiveMechanic, new()
    {
        var name = Registry<T>.Name;
        var text = to_ms_string(average_frame_times[name]);

        return (name, text);
    }

    private static string to_ms_string(double ticks) => (Math.Floor(ticks / (Stopwatch.Frequency / 1_000_000.0)) / 1000.0).ToString() + "ms";

    static class Registry<T> where T : class, new()
    {
        public static string Name => name ?? (name = typeof(T).Name);
        static string name;
        
        public static T Instance => instance ?? (instance = new T());
        static T instance;
    }

    public static double moving_average_weight = 0.5;
    static Stopwatch mechanics_sw = new Stopwatch();
    static Stopwatch frame_sw = new Stopwatch();
    static Stopwatch update_sw = new Stopwatch();

    static uint total_frames;
    static Dictionary<string, double> average_frame_times = new Dictionary<string, double>();
    static Dictionary<string, bool> average_frame_times_updated = new Dictionary<string, bool>();
    static double average_frame_time;
    static double average_update_time;
}