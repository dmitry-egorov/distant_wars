using Plugins.Lanski;
using UnityEngine;

public class find_and_attack_target : MassiveMechanic
{
    public void _()
    {
        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* units            */ var us  = ur.Units;
        /* map              */ var map = Map.Instance;

        /* projectiles manager */ var pm = ProjectilesManager.Instance;
        /* projectile shooter        */ var pshooter = pm.shooters;
        /* projectile positions      */ var pposs    = pm.positions;
        /* prev projectile positions */ var ppposs   = pm.prev_positions;
        /* projectile directions     */ var pdirs    = pm.directions;
        /* projectile speeds         */ var pspds    = pm.speeds;    
        /* projectile damages        */ var pdmgs    = pm.damages;
        /* hit radius                */ var hradius  = pm.HitRadius;
        /* predict target's position */ var predict  = ur.PredictTargetPosition;

        /* delta time */ var dt  = Game.Instance.DeltaTime;

        /* unit's space grid   */ var grid = ur.SpaceGrid;
        /* cells unit postions     */ var guposs  = grid.unit_positions;
        /* cells unit teams        */ var guteams = grid.unit_team_masks;
        /* cells unit visibilities */ var guviss  = grid.unit_detections;
        /* cells unit visibilities */ var gudiss  = grid.unit_discoveries;
        /* cells units             */ var gunits  = grid.unit_refs;
                    
        foreach (var unit in us)
        {
            /* attack cooldown     */ var ocd = unit.AttackCountdown;
            /* new attack cooldown */ var ncd = unit.AttackCountdown = ocd > 0 ? ocd - dt : 0;

            if (ncd > 0)
                // attack is on cooldown
                continue;

            /* unit's position 2d   */ var upos2d  = unit.Position;
            /* attack range         */ var range   = unit.AttackRange;
            /* target is found      */ var target_found = false;

            /* possible target unit */ 
            // check attack order target and last attack target
            if (unit.IssuedOrder.is_attack(/* attack order target */ out var ptarget)
             || unit.LastAttackTarget.try_get(/* last attack target */ out ptarget))
            {
                /* target's position 2d */
                var tp2 = ptarget.Position;
                /* target's offset      */
                var to = tp2 - upos2d;
                /* distance to target   */
                var td = to.sqrMagnitude;

                // target is within range
                if (td <= range.sqr())
                {
                    unit.LastAttackTarget = ptarget;
                    target_found = true;
                }
            }

            if (!target_found)
            // find first enemy in range
            {
                var uteam_mask = unit.Faction.Team.Mask;

                //PERF: can probably be done faster by walking outwards from the center
                /* attack range ^2        */ var range2 = range.sqr();
                /* min distance ^2 so far */ var mindst2 = float.MaxValue;
                /* grid attack area       */ var it = grid.get_iterator_of_circle(upos2d, range);

                while (it.next(out var cell_i))
                {
                    /* cell unit positions   */ var cuposs  = guposs [cell_i];
                    /* cell unit teams       */ var cuteams = guteams[cell_i];
                    /* cell unit visbilities */ var cuviss  = guviss [cell_i];
                    /* cell unit visbilities */ var cudiss  = gudiss [cell_i];
                    /* cell units refs       */ var curefs  = gunits [cell_i];
                    /* cell units count      */ var cucount = cuposs.Count;

                    for (/* target unit index */ int tunit_i = 0; tunit_i < cucount; tunit_i++)
                    {
                        /* target's team */ var tteam = cuteams[tunit_i];
                        if (tteam == uteam_mask) 
                            // target is on the same team
                            continue;

                        /* target's visibility */ var tvis = cuviss[tunit_i];
                        if ((tvis & uteam_mask) == 0) 
                            // target is not visible
                            continue;

                        /* target's discovery */ var tdis = cudiss[tunit_i];
                        if ((tdis & uteam_mask) == 0) 
                            // target is not discovered
                            continue;
                        
                        /* target's position 2d  */ var tpos2 = cuposs[tunit_i];
                        /* target's offset       */ var toff  = tpos2 - upos2d;
                        /* distance to target ^2 */ var tdist2 = toff.sqrMagnitude;
                        if (tdist2 < mindst2 && tdist2 < range2)
                        {
                            mindst2 = tdist2;
                            ptarget = curefs[tunit_i];
                            target_found = true;
                        }
                    }
                }
            }

            if (!target_found)
                // target not found
                continue;

            // calculate shot direction
            /* target's position 2d   */ var tpos2d  = ptarget.Position;
            /* target's prev position */ var tppos2d = ptarget.PrevPosition;
            /* projectiles speed      */ var pspeed  = unit.ProjectileSpeed;

            if (predict && tpos2d != tppos2d)
            // fire at predicted position
            {
                /* target's velocity  */ var tvel2d = (tpos2d - tppos2d) / dt;
                /* delta to target 2d */ var tdelta2d = tpos2d - upos2d;

                // solution for:
                // |tvel2d * t + tdelta2d| = pspeed * t
                // (tvel2d^2 - pspeed^2) * t^2 + 2 * dot(tvel2d, tdelta2d) * t + tdelta2d^2 = 0

                var tvel2d2 = tvel2d.sqrMagnitude;
                var pspeed2 = pspeed.sqr();
                var tdelta2d2 = tdelta2d.sqrMagnitude;

                var a  = tvel2d2 - pspeed2;
                var hb = Vector2.Dot(tvel2d, tdelta2d); // half b
                var c  = tdelta2d2;

                /* half determinant ^2 */ var hd2 = hb.sqr() - a * c;
                if (hd2 >= 0)
                {
                    var hd = Mathf.Sqrt(hd2);
                    var t1 = (-hb - hd) / a;
                    var t2 = (-hb + hd) / a;
                    if (t1 >= 0 || t2 >= 0)
                    {
                        var t = t2 < 0 ? t1
                              : t1 < 0 ? t2
                              : Mathf.Min(t1, t2)
                        ;

                        // assuming the same height offset
                        tpos2d += tvel2d * t;
                    }
                }
            }

            // create projectile
            /* unit's turret position */ var upos3d   = upos2d.xy(map.z(upos2d) + hradius);
            /* target's position   3d */ var tpos3d   = tpos2d.xy(map.z(tpos2d) + hradius);
            /* delta to target     3d */ var tdelta3d = tpos3d - upos3d;
            /* direction to target 3d */ var tdir3dn   = tdelta3d.normalized;

            /* projectile damage   */ var pdmg = unit.ProjectileDamage;

            pshooter.Add(unit);
            pposs .Add(upos3d);
            ppposs.Add(upos3d.xy());
            pdirs .Add(tdir3dn);
            pspds .Add(pspeed);
            pdmgs .Add(pdmg);

            // reset attack cooldown
            /* cooldown time */ var cdtime = unit.AttackCooldownTime;
            unit.AttackCountdown = ncd + cdtime;
        }
    }
}