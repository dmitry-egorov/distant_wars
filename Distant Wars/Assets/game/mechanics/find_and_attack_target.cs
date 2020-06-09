using Plugins.Lanski;
using UnityEngine;

public class find_and_attack_target : IMassiveMechanic
{
    public void _()
    {
        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* units            */ var us  = ur.all_units;
        /* map              */ var map = Map.Instance;

        /* projectiles manager       */ var pm = ProjectilesManager.Instance;
        /* projectile shooter        */ var proj_shooters = pm.shooters;
        /* projectile positions      */ var proj_poss = pm.positions;
        /* prev projectile positions */ var proj_prev_poss = pm.prev_positions;
        /* projectile directions     */ var proj_dirs = pm.directions;
        /* projectile speeds         */ var proj_speeds = pm.speeds;    
        /* projectile damages        */ var proj_damages = pm.damages;
        
        /* hit radius                */ var hit_radius  = pm.HitRadius;
        /* predict target's position */ var predict_target_pos  = ur.PredictTargetPosition;

        /* delta time */ var dt  = Game.Instance.DeltaTime;

        /* unit's space grid */ var grid = ur.all_units_grid;
        /* cells unit postions        */ var grid_unit_poss = grid.unit_positions;
        /* cells unit teams           */ var grid_unit_teams = grid.unit_teams;
        /* cells unit detections      */ var grid_unit_detects = grid.unit_detections_by_team;
        /* cells unit identifications */ var grid_unit_idents = grid.unit_identifications_by_team;
        /* cells units                */ var grid_unit_refs = grid.unit_refs;
                    
        foreach (var unit in us)
        {
            /* attack cooldown     */ var old_attack_cd = unit.attack_remaining_cooldown;
            /* new attack cooldown */ var new_attack_cd = old_attack_cd > 0 ? old_attack_cd - dt : 0;
            unit.attack_remaining_cooldown = new_attack_cd;

            if (new_attack_cd > 0)
                // attack is on cooldown
                continue;

            /* unit's position 2d   */ var unit_pos_2d  = unit.position;
            /* unit's attack range  */ var u_range   = unit.AttackRange;
            /* target is found      */ var target_found = false;

            /* possible target unit */ 
            // check attack order target and last attack target
            if (unit.issued_order.is_attack(/* attack order target */ out var target)
             || unit.last_attack_target.try_get(/* last attack target */ out target))
            {
                /* target's position 2d */ var tp2 = target.position;
                /* target's offset      */ var to = tp2 - unit_pos_2d;
                /* distance to target   */ var td = to.sqrMagnitude;

                // target is within range
                if (td <= u_range.sqr())
                {
                    unit.last_attack_target = target;
                    target_found = true;
                }
            }

            if (!target_found)
            // find first enemy in range
            {
                var unit_team = unit.Faction.Team.Mask;

                //PERF: can probably be done faster by walking outwards from the center
                /* attack range ^2        */ var range_sq = u_range.sqr();
                /* min distance ^2 so far */ var min_dst_sq = float.MaxValue;
                /* grid attack area       */ var it = grid.get_iterator_of_circle(unit_pos_2d, u_range);

                while (it.next(out var cell_i))
                {
                    /* cell unit positions       */ var cell_unit_poss = grid_unit_poss[cell_i];
                    /* cell unit teams           */ var cell_unit_teams = grid_unit_teams[cell_i];
                    /* cell unit detections      */ var cell_unit_detects = grid_unit_detects[cell_i];
                    /* cell unit identifications */ var cell_unit_idents = grid_unit_idents[cell_i];
                    /* cell units refs           */ var cell_unit_refs = grid_unit_refs[cell_i];
                    /* cell units count          */ var cell_unit_count = cell_unit_poss.Count;

                    for (var target_i = 0; target_i < cell_unit_count; target_i++)
                    {
                        if (cell_unit_teams[target_i] == unit_team) 
                            // target is on the same team
                            continue;

                        if ((cell_unit_detects[target_i] & unit_team) == 0) 
                            // target is not detected
                            continue;

                        if ((cell_unit_idents[target_i] & unit_team) == 0) 
                            // target is not identified
                            continue;
                        
                        /* target's position 2d  */ var target_pos_2d = cell_unit_poss[target_i];
                        /* target's offset       */ var target_off  = target_pos_2d - unit_pos_2d;
                        /* distance to target ^2 */ var target_dist_sq = target_off.sqrMagnitude;
                        if (target_dist_sq < min_dst_sq && target_dist_sq < range_sq)
                        {
                            min_dst_sq = target_dist_sq;
                            target = cell_unit_refs[target_i];
                            target_found = true;
                        }
                    }
                }
            }

            if (target_found)
            // fire at the target 
            {
                // calculate shot direction
                /* target's position 2d   */ var target_pos_2d  = target.position;
                /* target's prev position */ var target_prev_pos_2d = target.prev_position;
                /* projectiles speed      */ var proj_speed  = unit.ProjectileSpeed;

                if (predict_target_pos && target_pos_2d != target_prev_pos_2d)
                // predict target position
                {
                    /* target's velocity  */ var target_vel_2d = (target_pos_2d - target_prev_pos_2d) / dt;
                    /* delta to target 2d */ var target_delta_2d = target_pos_2d - unit_pos_2d;

                    // solution for:
                    // |tvel2d * t + tdelta2d| = pspeed * t
                    // (tvel2d^2 - pspeed^2) * t^2 + 2 * dot(tvel2d, tdelta2d) * t + tdelta2d^2 = 0

                    var target_vel_2d_sq = target_vel_2d.sqrMagnitude;
                    var proj_speed_sq = proj_speed.sqr();
                    var target_dist_2d = target_delta_2d.sqrMagnitude;

                    var a  = target_vel_2d_sq - proj_speed_sq;
                    var hb = Vector2.Dot(target_vel_2d, target_delta_2d); // half b
                    var c  = target_dist_2d;

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
                            target_pos_2d += target_vel_2d * t;
                        }
                    }
                }

                // create projectile
                /* unit's turret position */ var unit_pos_3d     = unit_pos_2d.xy(map.z(unit_pos_2d) + hit_radius);
                /* target's position   3d */ var target_pos_3d   = target_pos_2d.xy(map.z(target_pos_2d) + hit_radius);
                /* delta to target     3d */ var target_delta_3d = target_pos_3d - unit_pos_3d;
                /* direction to target 3d */ var target_dir_3d   = target_delta_3d.normalized;

                /* projectile damage   */ var proj_dmg = unit.ProjectileDamage;

                proj_shooters .Add(unit);
                proj_poss     .Add(unit_pos_3d);
                proj_prev_poss.Add(unit_pos_3d.xy());
                proj_dirs     .Add(target_dir_3d);
                proj_speeds   .Add(proj_speed);
                proj_damages  .Add(proj_dmg);

                // reset attack cooldown
                /* cooldown time */ var cd = unit.AttackCooldownTime;
                unit.attack_remaining_cooldown = new_attack_cd + cd;
            }
        }
    }
}