#if UNITY_EDITOR
internal class editor_set_position_from_transform : IMassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var us = ur.all_units;
        
        foreach (var u in us)
            u.prev_position = u.position = u.transform.position.xy();
    }
}
#endif