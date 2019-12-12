using Plugins.Lanski;

public class cleanup_units : MassiveMechanic
{
    public void _() 
    {
        var ur = UnitsRegistry.Instance;
        ur.Units.CleanUpExpiredObjects();
        ur.OwnTeamUnits.CleanUpExpiredObjects();
    }
}