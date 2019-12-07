using Plugins.Lanski;
using UnityEngine;

public class cleanup_units : MassiveMechanic
{
    public void _() 
    {
        var ur = UnitsRegistry.Instance;
        var c = ur.Units.clean_up_dead_objects();
        ur.VisionUnits.clean_up_dead_objects();
        ur.OtherUnits.clean_up_dead_objects();

        if (c > 0)
        {
            Debug.Log($"Cleaned {c} units");
        }
    }
}