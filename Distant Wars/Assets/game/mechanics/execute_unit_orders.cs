using UnityEngine;

public class execute_unit_orders : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        
        var /* units             */ us = ur.Units;
        var /* map               */  m = Map.Instance;
        var /* bullet's manager  */ bm = BulletsManager.Instance;
        var /* easing distance   */ ed = 2f;
        var /* stopping distance */ sd = 0.5f;
        var dt = Time.deltaTime;

        foreach (var u in us)
        {
            var p = u.Position;

            // execute attack order
            if (u.AttackTarget.try_get(out var at))
            {
                var ar = u.AttackRange;
                var tp = at.Position;
                var off = (tp - p);
                var om = off.magnitude;

                if (om > ar)
                {
                    u.MoveTarget = tp;
                }
                else if (u.AttackCooldownCountdown <= 0.0)
                {
                    var /* direction */ d = off / om;
                    bm.Positions.Add(p + d);
                    bm.Velocities .Add(d * u.AttackVelocity);
                    bm.Damages   .Add(u.AttackDamage);

                    u.AttackCooldownCountdown = u.AttackCooldownTime;
                }
            }

            // execute move order
            if (u.MoveTarget.try_get(out var /* target */ mt))
            {
                var d = (p - mt).magnitude;
                if (d > sd)
                {
                    var /* speed */ s = u.BaseSpeed;
                    float slope = -2000.0f * Mathf.Max(m.slope2(p, (mt - p).normalized), 0.0f);
                    var /* terrain speed */ ts = s * Mathf.Exp(slope);
                    var /* smoothed speed */ ss = ts * Mathf.Clamp01(d / ed);
                    u.Position = Vector2.MoveTowards(p, mt, ss * dt);
                }
                else
                {
                    u.MoveTarget = default;
                }
            }
        }
    }
}