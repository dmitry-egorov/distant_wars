using System.Collections.Generic;
using UnityEngine;

internal class set_position_from_transform_in_editor : MassiveMechanic
{
    public void _()
    {
        if (Application.isPlaying)
            return;

        var ur = UnitsRegistry.Instance;
        set_position(ur.OwnUnits);
        set_position(ur.OtherUnits);
    }

    private static void set_position(List<Unit> us)
    {
        foreach (var u in us)
            u.Position = u.transform.position.xy();
    }
}