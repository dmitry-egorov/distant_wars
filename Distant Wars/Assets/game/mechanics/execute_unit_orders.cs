using System.Collections.Generic;
using UnityEngine;

public class execute_unit_orders : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        move_units(ur.OwnUnits);
        move_units(ur.OtherUnits);
    }

    private static void move_units(List<Unit> us)
    {
        var /* map               */  m = Map.Instance;
        var /* easing distance   */ ed = 2f;
        var /* stopping distance */ sd = 0.5f;
        var dt = Time.deltaTime;

        foreach (var u in us)
        {
            if (!u.MoveTarget.TryGetValue(out var /* target */ t))
                continue;

            var p = u.Position;
            var d = (p - t).magnitude;
            if (d > sd)
            {
                var /* speed */ s = u.BaseSpeed;
                float slope = -2000.0f * Mathf.Max(m.slope2(p, (t - p).normalized), 0.0f);
                var /* terrain speed */ ts = s * Mathf.Exp(slope);
                var /* smoothed speed */ ss = ts * Mathf.Clamp01(d / ed);
                u.Position = Vector2.MoveTowards(p, t, ss * dt);
            }
            else
            {
                u.MoveTarget = default;
            }
        }
    }
}