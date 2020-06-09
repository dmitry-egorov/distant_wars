using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

public class MassiveGame<TGame>: RequiredSingleton<TGame> where TGame: MassiveGame<TGame>
{
    [Header("Settings")]
    public float CurrentTimeMeasuringWeight = 0.5f;
    public float AverageTimeMeasuringWeight = 0.01f;
    public float SteadySimulationFrameRate  = 20.0f;

    [Header("State")]
    public float DeltaTime;
    public float PresentationToSimulationFrameTimeRatio;
    public double PresentationTotalTime;
    public double SteadySimulationTotalTime;

    public void run<TMechanic>() where TMechanic : class, IMassiveMechanic, new()
    {
        mechanics_sw.Restart();
        var name = Registry<TMechanic>.Name;
        Profiler.BeginSample(name);
        try
        {
            Registry<TMechanic>.Instance._();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            MassiveDebug.Display(e.ToString());
        }
        finally
        {
            var ticks = mechanics_sw.ElapsedTicks;
            if (!frame_time_measures.TryGetValue(name, out var measure))
            {
                frame_time_measures[name] = measure = new TimeMeasure {current = ticks, average = ticks, is_updated_this_frame = true};
            }

            var cw = CurrentTimeMeasuringWeight;
            var aw = AverageTimeMeasuringWeight;

            measure.current = measure.current * (1.0 - cw) + cw * ticks;
            measure.average = measure.average * (1.0 - aw) + aw * ticks;
            measure.is_updated_this_frame = true;

            Profiler.EndSample();
        }
    }

    public void game_loop(Action init, Action handle_input, Action simulate, Action<bool> render)
    {
        // initialisation
        if (!initialized)
        {
            mechanics_sw = new Stopwatch();
            frame_sw = new Stopwatch();
            update_sw = new Stopwatch();
            frame_time_measures = new Dictionary<string, TimeMeasure>();

            init();
            
            initialized = true;
        }

        update_sw.Restart();
        var steady_delta_time = 1 / SteadySimulationFrameRate;

        // update input
        {
            DeltaTime = Time.deltaTime;
            PresentationTotalTime += DeltaTime;

            handle_input();
        }

        var had_at_least_one_steady_update = false;
        // update simulation
        {
            DeltaTime = steady_delta_time;
            while (SteadySimulationTotalTime < PresentationTotalTime)
            {
                simulate();

                SteadySimulationTotalTime += DeltaTime;
                had_at_least_one_steady_update = true;
            }
        }

        // presentation
        {
            DeltaTime = Time.deltaTime;
            PresentationToSimulationFrameTimeRatio = (float)((PresentationTotalTime - (SteadySimulationTotalTime - steady_delta_time)) / steady_delta_time);

            render(had_at_least_one_steady_update);
        }

        // measuring update time
        {
            var frame_elapsed = frame_sw.ElapsedTicks;
            var update_elapsed = update_sw.ElapsedTicks;

            var cw = CurrentTimeMeasuringWeight;
            var aw = AverageTimeMeasuringWeight;

            current_frame_time = (1.0 - cw) * current_frame_time + cw * frame_elapsed;
            average_frame_time = (1.0 - aw) * average_frame_time + aw * frame_elapsed;
            frame_sw.Restart();

            current_update_time = (1.0 - cw) * current_update_time + cw * update_elapsed;
            average_update_time = (1.0 - aw) * average_update_time + aw * update_elapsed;

            var measures = frame_time_measures.Values.ToArray();
            foreach (var measure in measures)
            {
                if (!measure.is_updated_this_frame)
                {
                    measure.current = measure.current * (1.0 - cw);
                    measure.average = measure.average * (1.0 - aw);
                }
                else
                {
                    measure.is_updated_this_frame = false;
                }
            }
        }
    }

    public (double avg, double cur) get_avg_frame_time()  => (average_frame_time, current_frame_time);
    public (double avg, double cur) get_avg_update_time() => (average_update_time, current_update_time);

    public IEnumerable<(string name, double avg, double cur)> get_top_avg_times(int count) => 
        frame_time_measures
            .OrderByDescending(x => x.Value.average)
            .Take(count)
            .Select(x => (x.Key, x.Value.average, x.Value.current))
        ;

    static class Registry<T> where T : class, new()
    {
        public static string Name => name ?? (name = typeof(T).Name);
        static string name;
        
        public static T Instance => instance ?? (instance = new T());
        static T instance;
    }

    class TimeMeasure
    {
        public double current;
        public double average;
        public bool is_updated_this_frame;
    }

    [NonSerialized] Stopwatch mechanics_sw;
    [NonSerialized] Stopwatch frame_sw;
    [NonSerialized] Stopwatch update_sw;

    [NonSerialized] Dictionary<string, TimeMeasure> frame_time_measures;
    [NonSerialized] double current_frame_time;
    [NonSerialized] double current_update_time;
    [NonSerialized] double average_frame_time;
    [NonSerialized] double average_update_time;
    [NonSerialized] bool initialized;
}