using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public abstract class MassiveRegistry<TM, T> : RequiredSingleton<TM> 
    where TM : MassiveRegistry<TM, T> 
    where T: MassiveBehaviour<TM, T>
{
    public List<T> All => objects ?? (objects = new List<T>());

    public void clean_up()
    {
        var i = 0;
        var list = All;
        while(i < list.Count)
        {
            if (list[i] == null)
            {
                var last_index = list.Count - 1;
                list[i] = list[last_index];
                list.RemoveAt(last_index);
            }
            else
            {
                i++;
            }
        }
    }

    internal void register(T b)
    {
        All.Add(b);
    }

    static List<T> objects;
}