using UnityEngine;

public class execute_unit_orders : MassiveMechanic
{
    public void _()
    {
        var /* units */ us = UnitsRegistry.Instance.Units;
        var /* easing distance */ ed = 2f;
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
                var /* smoothed speed */ ss = s * Mathf.Clamp01(d / ed);
                u.Position = Vector2.MoveTowards(p, t, ss * dt);
            }
            else
            {
                u.MoveTarget = default;
            }
        }
    }
}