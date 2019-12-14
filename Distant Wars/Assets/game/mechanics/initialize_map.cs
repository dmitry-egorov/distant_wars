using Plugins.Lanski;

internal class init_map : MassiveMechanic
{
    public void _()
    {
        var m = Map.Instance;
        m.initialize();
    }
}