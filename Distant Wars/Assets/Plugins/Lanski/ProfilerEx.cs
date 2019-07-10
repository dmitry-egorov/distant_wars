using System;
using UnityEngine.Profiling;

public static class ProfilerEx
{
    public static void Sample(string name, Action a)
    {
        Profiler.BeginSample(name);
        try
        {
            a();
        }
        finally
        {
            Profiler.EndSample();
        }
    }
}