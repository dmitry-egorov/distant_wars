using Plugins.Lanski;
using UnityEngine;

public class handle_unit_attacking : MassiveMechanic
{
    public void _()
    {
        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* units            */ var us  = ur.Units;
        /* map              */ var map = Map.Instance;
        /* bullet's manager */ var pm  = ProjectilesManager.Instance;
        /* delta time       */ var dt  = Time.deltaTime;

        foreach (var u in us)
        {
            /* attack cooldown     */ var ocd  = u.AttackCountdown;
            /* new attack cooldown */ var ncd = u.AttackCountdown = ocd > 0 ? ocd - dt : 0;

            if (ncd > 0)
                continue;

            /* unit's position 2d   */ var up2 = u.Position;
            /* attack range         */ var ar  = u.AttackRange;
            /* possible target unit */ var ptu = default(Unit);

            // check attack order target
            if (u.IssuedOrder.is_attack(/* attack order target */ out var aot))
            {
                try_set_attack_target(aot);
            }
            // check last attack target
            else if (u.LastAttackTarget.try_get(/* last attack target */ out var lat))
            {
                try_set_attack_target(lat);
            }
            // find first enemy in range
            else
            {
                /* unit's faction */ var uf = u.Faction;
                //PERF: use space grid
                foreach (/* other unit */ var ou in us)
                {
                    if (ou.Faction == uf) continue;
                    if (try_set_attack_target(ou)) break;
                }
            }

            if (ptu != null)
            {
                /* projectile's height    */ var ph = u.ProjectileHeight;
                /* projectile offset      */ var po = u.ProjectileOffset;
                /* projectile velocity    */ var av = u.AttackVelocity;
                /* projectile damage      */ var ad = u.AttackDamage;
                /* cooldown time          */ var cd = u.AttackCooldownTime;
                
                /* target's position 2d   */ var tp2 = ptu.Position;
                /* target's position 3d   */ var tp3 = map.xyz(tp2);
                /* unit's turret position */ var utp = up2.xy(map.z(up2) + ph);
                /* target's offset        */ var to  = tp3 - utp;
                /* distance to target     */ var td  = to.magnitude;
                /* projectile's direction */ var d  = to / td;
                /* projectile's position  */ var pp = utp + d * po;
                pm.Positions .Add(pp);
                pm.Directions.Add(d);
                pm.Speeds    .Add(av);
                pm.Damages   .Add(ad);

                u.AttackCountdown = ncd + cd;
            }

            bool try_set_attack_target(/* target unit */ Unit ou)
            {
                /* target's position 2d */ var tp2 = ou.Position;
                /* target's offset      */ var to  = tp2 - up2;
                /* distance to target   */ var td  = to.sqrMagnitude;

                if (td <= ar.sqr())
                {
                    ptu = ou;
                    u.LastAttackTarget = ptu;
                    return true;
                }

                return false;
            }
        }

        
    }
}