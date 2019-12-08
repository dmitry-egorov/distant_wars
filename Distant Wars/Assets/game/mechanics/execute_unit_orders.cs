using Plugins.Lanski;
using UnityEngine;

public class execute_unit_orders : MassiveMechanic
{
    private const float GuardApproachDistance = 2.0f;
    private const float AttackApproachDistanceMargin = 5.0f;

    public void _()
    {
        /* units' registry */ var ur = UnitsRegistry.Instance;
        /* units           */ var us = ur.Units;
        /* map             */ var m  = Map.Instance;

        // attack
        /* bullet's manager */ var pm = ProjectilesManager.Instance;
        foreach (var u in us)
        {
            /* attack cooldown */ var acd = u.AttackCountdown;

            if (acd > 0)
                continue;

            /* unit's position 2d   */ var up2 = u.Position;
            /* attack range         */ var ar  = u.AttackRange;
            /* possible target unit */ var ptu = default(Unit);

            // check attack order target
            if (u.IssuedOrder.is_attack(/* attack order target */ out var aot))
            {
                if (aot == null)
                    u.IssuedOrder = Unit.Order.idle(); // clear empty attack order (happens when target is destroyed)
                else 
                    try_set_attack_target(aot);
            }

            // check last attack target
            if (ptu == null && u.LastAttackTarget.try_get(/* last attack target */ out var lat))
            {
                try_set_attack_target(lat);
            }

            /* unit's faction */

            // find first enemy in range
            if (ptu == null)
            {
                var uf = u.Faction;

                //PERF: use spacial structure
                foreach(/* other unit */ var ou in us)
                {
                    if (ou.Faction == uf)
                        continue;

                    try_set_attack_target(ou);
                }
            }

            if (ptu.try_get(/* target unit */ out var tu))
            {
                /* projectile's height    */ var ph = u.ProjectileHeight;
                /* projectile offset      */ var po = u.ProjectileOffset;
                /* projectile velocity    */ var av = u.AttackVelocity;
                /* projectile damage      */ var ad = u.AttackDamage;
                /* cooldown time          */ var cd = u.AttackCooldownTime;
                
                /* target's position 2d   */ var tp2 = tu.Position;
                /* target's position 3d   */ var tp3 = m.xyz(tp2);
                /* unit's turret position */ var utp = up2.xy(m.z(up2) + ph);
                /* target's offset        */ var to  = tp3 - utp;
                /* distance to target     */ var td  = to.magnitude;
                /* projectile's direction */ var d  = to / td;
                /* projectile's position  */ var pp = utp + d * po;
                pm.Positions .Add(pp);
                pm.Directions.Add(d);
                pm.Speeds    .Add(av);
                pm.Damages   .Add(ad);

                u.AttackCountdown += cd;
                break;
            }

            void try_set_attack_target(/* target unit */ Unit ou)
            {
                /* target's position 2d */ var tp2 = ou.Position;
                /* target's offset      */ var to  = tp2 - up2;
                /* distance to target   */ var td  = to.sqrMagnitude;

                if (td <= ar.sqr())
                {
                    ptu = ou;
                    u.LastAttackTarget = ptu;
                }
            }
        }

        // move
        /* easing distance   */ var ed = 2f;
        /* stopping distance */ var sd = 0.5f;
        /* delta time        */ var dt = Time.deltaTime;
        /* guard approach distance */ var gad = GuardApproachDistance;
        /* attack approach distance margin */ var aadm = AttackApproachDistanceMargin;

        foreach (var u in us)
        {
            /* unit's position 2d   */ var up  = u.Position;
            /* possible move target */ var pmt = default(Vector2?);

            var is_move_order = u.IssuedOrder.is_move(out var mo);

            if (is_move_order)
            {
                pmt = mo;
            }
            else 
            {
                var ot = default(Unit);
                var igo = u.IssuedOrder.is_guard(out ot);
                var iao = !igo ? u.IssuedOrder.is_attack(out ot) : false;

                if (igo || iao)
                {
                    /* approach distance  */ var ad = igo ? gad : u.AttackRange - aadm;
                    /* target's position  */ var tp = ot.Position;
                    /* target's offset    */ var to = tp - up;
                    /* distance to target */ var td = to.magnitude;
                    /* distance remaining */ var dr = td - ad;

                    if (dr > 0)
                    {
                        /* direction towards target */ var tdir = to / td;
                        /* approach point           */ var ap = up + dr * tdir;
                        pmt = ap;
                    }
                }
            }

            if (pmt.try_get(out var mt))
            {
                var d = (up - mt).magnitude;
                if (d > sd)
                {
                    /* speed */ var sp = u.BaseSpeed;
                    /* slope */ var sl = -0.5f * Mathf.Max(m.slope2(up, (mt - up).normalized), 0.0f);
                    /* terrain speed  */ var ts = sp * Mathf.Exp(sl);
                    /* smoothed speed */ var ss = ts * Mathf.Clamp01(d / ed);
                    u.Position = Vector2.MoveTowards(up, mt, ss * dt);
                }
                else if (is_move_order)
                {
                    u.IssuedOrder = Unit.Order.idle();
                }
            }
        }
    }
}