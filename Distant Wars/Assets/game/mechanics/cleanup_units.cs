public class cleanup_units : MassiveMechanic
{
    public void _() => UnitsRegistry.Instance.clean_up();
}