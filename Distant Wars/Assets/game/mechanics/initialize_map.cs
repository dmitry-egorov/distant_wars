using Plugins.Lanski;

internal class initialize_map : MassiveMechanic
{
    public void _()
    {
        var m = Map.Instance;
        m.transform.localScale = m.Scale.v3();
    }
}