using Plugins.Lanski;

public class cleanup_units : MassiveMechanic
{
    public void _() 
    {
        var ur = UnitsRegistry.Instance;
        var c = ur.Units.CleanUpExpiredObjects();
        ur.OwnTeamUnits.CleanUpExpiredObjects();
        ur.OtherTeamsUnits.CleanUpExpiredObjects();

        //if (c > 0) Debug.Log($"Cleaned {c} units");
    }
}