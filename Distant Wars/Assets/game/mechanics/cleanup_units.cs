using Plugins.Lanski;

public class cleanup_units : MassiveMechanic
{
    public void _() 
    {
        var ur = UnitsRegistry.Instance;
        var c = ur.Units.clean_up_expired_objects();
        ur.VisionUnits.clean_up_expired_objects();
        ur.OtherUnits.clean_up_expired_objects();

        //if (c > 0) Debug.Log($"Cleaned {c} units");
    }
}