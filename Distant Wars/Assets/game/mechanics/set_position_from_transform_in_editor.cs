#if UNITY_EDITOR
internal class editor_set_position_from_transform : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var us = ur.Units;
        
        foreach (var u in us)
            u.PrevPosition = u.Position = u.transform.position.xy();
    }
}
#endif