using System.Collections.Generic;
using Plugins.Lanski;

public abstract class MassiveRegistry<TM, T> : RequiredSingleton<TM> 
    where TM : MassiveRegistry<TM, T> 
    where T: MassiveBehaviour<TM, T>
{
    public List<T> NewObjects => new_objects ?? (new_objects = new List<T>());

    internal void register(T b)
    {
        NewObjects.Add(b);
    }

    static List<T> objects;
    static List<T> new_objects;
}