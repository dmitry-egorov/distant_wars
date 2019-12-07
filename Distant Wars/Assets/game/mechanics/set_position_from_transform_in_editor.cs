using UnityEngine;

internal class set_position_from_transform_in_editor : MassiveMechanic
{
    public void _()
    {
        if (Application.isPlaying)
            return;

        var ur = UnitsRegistry.Instance;
        var us = ur.Units;
        
        foreach (var u in us)
            u.Position = u.transform.position.xy();
    }
}