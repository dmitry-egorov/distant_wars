using Plugins.Lanski;

public class find_and_attack_target : MassiveMechanic
{
    public void _()
    {
        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* units            */ var us  = ur.Units;
        /* map              */ var map = Map.Instance;

        /* projectiles manager */ var pm  = ProjectilesManager.Instance;
        /* projectile positions      */ var pposs  = pm.positions;
        /* prev projectile positions */ var ppposs = pm.prev_positions;
        /* projectile directions     */ var pdirs  = pm.directions;
        /* projectile speeds         */ var pspds  = pm.speeds;    
        /* projectile damages        */ var pdmgs  = pm.damages;
        /* delta time       */ var dt  = Game.Instance.DeltaTime;

        /* unit's space grid   */ var grid = ur.SpaceGrid;
        /* cells unit postions     */ var guposs  = grid.unit_positions;
        /* cells unit teams        */ var guteams = grid.unit_team_masks;
        /* cells unit visibilities */ var guviss  = grid.unit_visibilities;
        /* cells units             */ var gunits  = grid.unit_refs;
                    
        foreach (var u in us)
        {
            /* attack cooldown     */ var ocd = u.AttackCountdown;
            /* new attack cooldown */ var ncd = u.AttackCountdown = ocd > 0 ? ocd - dt : 0;

            if (ncd > 0)
                continue;

            /* unit's position 2d   */ var up2 = u.Position;
            /* attack range         */ var ar  = u.AttackRange;
            /* possible target unit */ var ptarget = default(Unit);

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
                /* grid attack area       */ var it = grid.get_iterator_of_circle(up2, ar);

                while (it.next(out var cell_i))
                {
                    /* cell unit positions   */ var cuposs  = guposs [cell_i];
                    /* cell unit teams       */ var cuteams = guteams[cell_i];
                    /* cell unit visbilities */ var cuviss  = guviss [cell_i];
                    /* cell units refs       */ var curefs  = gunits [cell_i];
                    /* cell units count      */ var cucount = cuposs.Count;

                    for (/* target unit index */ int tunit_i = 0; tunit_i < cucount; tunit_i++)
                    {
                        /* target's team */ var tteam = cuteams[tunit_i];
                        // target is on the same team
                        if (tteam == uteam_mask) continue;

                        /* target's visibility */ var tvis = cuviss[tunit_i];
                        // target is not visible
                        if ((tvis & uteam_mask) == 0) continue;

                        /* target's position 2d  */ var tpos2 = cuposs[tunit_i];
                        /* target's offset       */ var toff  = tpos2 - up2;
                        /* distance to target ^2 */ var tdist2 = toff.sqrMagnitude;
                        if (tdist2 < mindst2 && tdist2 < ar2)
                        {
                            mindst2 = tdist2;
                            ptarget = curefs[tunit_i];
                        }
                    }
                }
            }

            if (ptarget != null)
            {
                /* projectile's height */ var pheight = u.ProjectileHeight;
                /* projectile offset   */ var poff = u.ProjectileOffset;
                /* projectile velocity */ var pvel = u.AttackVelocity;
                /* projectile damage   */ var pdmg = u.AttackDamage;
                /* cooldown time       */ var cd = u.AttackCooldownTime;
                
                /* target's position 2d   */ var tp2 = ptarget.Position;
                /* target's position 3d   */ var tp3 = map.xyz(tp2);
                /* unit's turret position */ var utp = up2.xy(map.z(up2) + pheight);
                /* target's offset        */ var to  = tp3 - utp;
                /* distance to target     */ var td  = to.magnitude;
                /* projectile's direction */ var dir   = to / td;
                /* projectile's position  */ var ppos  = utp + dir * poff;

                // create projectile
                pposs .Add(ppos);
                ppposs.Add(ppos.xy());
                pdirs .Add(dir);
                pspds .Add(pvel);
                pdmgs .Add(pdmg);

                // reset attack cooldown
                u.AttackCountdown = ncd + cd;
            }

            bool try_set_attack_target(/* target unit */ Unit tunit)
            {
                /* target's position 2d */ var tp2 = tunit.Position;
                /* target's offset      */ var to  = tp2 - up2;
                /* distance to target   */ var td  = to.sqrMagnitude;

                // target is within range
                if (td <= ar.sqr())
                {
                    ptarget = tunit;
                    u.LastAttackTarget = ptarget;
                    return true;
                }

                return false;
            }
        }
    }
}