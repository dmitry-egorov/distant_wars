using System;
using UnityEngine;

public class prepare_units_scale : MassiveMechanic
{
    public void _()
    {
        var /* strategic camera */ sc = StrategicCamera.Instance;
        var /* units' registry */  ur = UnitsRegistry.Instance;
        
        var      /* unit's screen size */   us = Math.Max(ur.UnitScreenSize * Screen.height / 1080, 1) * 32; // order of operations is important for rounding
        var /* screen space multiplier */  s2w = sc.ScreenToWorldSpaceMultiplier;
        var              /* world size */   ws = us * s2w;
        var         /* min unit's size */ mins = ur.MinWorldSize;
        var           /* adjusted size */    s = Mathf.Max(mins, ws);

        UnitsRegistry.Instance.WorldScale = s;
    }
}