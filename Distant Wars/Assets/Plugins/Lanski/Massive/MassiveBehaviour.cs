using System.Collections.Generic;
using UnityEngine;

public abstract class MassiveBehaviour<T>: MonoBehaviour
    where T: MassiveBehaviour<T>
{
    void OnEnable()
    {
        All.Add((T)this);
    }
    
    public static List<T> All => objects ?? (objects = new List<T>());

    public static void CleanUpAll()
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

    static List<T> objects;
}