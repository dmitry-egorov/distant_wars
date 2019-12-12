using Plugins.Lanski;
using UnityEngine;

public class find_and_attack_target : MassiveMechanic
{
    public void _()
    {
        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* units            */ var us  = ur.Units;
        /* map              */ var map = Map.Instance;
        /* bullet's manager */ var pm  = ProjectilesManager.Instance;
        /* delta time       */ var dt  = Time.deltaTime;

        /* unit's space grid   */ var usg  = ur.SpaceGrid;
        /* cells unit postions */ var csps = usg.unit_positions;
        /* cells unit postions */ var ctis = usg.unit_team_masks;
        /* cells units         */ var csus = usg.unit_refs;
        /* cells centers       */ var cscs = usg.cell_centers;
                    
        foreach (var u in us)
        {
            /* attack cooldown     */ var ocd = u.AttackCountdown;
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
                var uteam_mask = u.Faction.Team.Mask;

                //PERF: can probably be done faster by walking outwards from the center
                /* attack range ^2        */ var ar2 = ar.sqr();
                /* min distance ^2 so far */ var mindst2 = float.MaxValue;
                /* grid attack area       */ var gaa = usg.get_coord_rect_of_circle(up2, ar);

                var minx = gaa.min.x;
                var miny = gaa.min.y;
                var maxx = gaa.max.x;
                var maxy = gaa.max.y;

                for (var yi = miny; yi <= maxy; yi++)
                for (var xi = minx; xi <= maxx; xi++)
                {
                    var ci = usg.get_index_of(xi, yi);
                    /* cell positions   */ var cps = csps[ci];
                    /* cell positions   */ var cts = ctis[ci];
                    /* cell units       */ var cus = csus[ci];
                    /* cell units count */ var cuc = cps.Count;

                    for (int i = 0; i < cuc; i++)
                    {
                        /* target's team */ var tt = cts[i];
                        if (tt == uteam_mask) continue;

                        //TODO: must be visible by the unit's team

                        /* target's position 2d  */ var tp2 = cps[i];
                        /* target's offset       */ var to  = tp2 - up2;
                        /* distance to target ^2 */ var td2 = to.sqrMagnitude;
                        if (td2 < mindst2 && td2 < ar2)
                        {
                            mindst2 = td2;
                            ptu = cus[i];
                        }
                    }
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
                /* projectile's direction */ var d   = to / td;
                /* projectile's position  */ var pp  = utp + d * po;
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