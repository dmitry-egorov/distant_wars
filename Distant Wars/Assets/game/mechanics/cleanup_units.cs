using Plugins.Lanski;

public class cleanup_units : MassiveMechanic
{
    public void _() 
    {
        var ur = UnitsRegistry.Instance;
        ur.OwnUnits.clean_up_dead_objects();
        ur.OtherUnits.clean_up_dead_objects();
    }
}