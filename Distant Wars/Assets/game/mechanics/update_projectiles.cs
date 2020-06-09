using Plugins.Lanski;
using UnityEngine;

internal class update_projectiles : IMassiveMechanic
{
    public void _()
    {
        var pm  = ProjectilesManager.Instance;
        /* shooters       */ var proj_shooters = pm.shooters;
        /* positions      */ var proj_poss = pm.positions;
        /* prev positions */ var proj_prev_poss = pm.prev_positions;
        /* directions     */ var proj_dirs = pm.directions;
        /* speeds         */ var proj_speeds = pm.speeds;
        /* damages        */ var proj_damages = pm.damages;
        
        /* count          */ var proj_count = proj_poss.Count;
        /* hit radius     */ var hit_radius = pm.HitRadius;

        /* delta time            */ var dt = Game.Instance.DeltaTime;
        /* map                   */ var map = Map.Instance;
        /* map 2 world transform */ var m2w = map.map_to_world;
        /* units registry        */ var ur = UnitsRegistry.Instance;
        /* units' space grids    */ var grid = ur.all_units_grid;
        /* grid's unit positions */ var unit_poss = grid.unit_positions;
        /* grid's units          */ var unit_refs = grid.unit_refs;
        /* bounding radius ^2    */ var bound_radius_sqr = map.BoundingRadius.sqr();

        var em = ExplosionsManager.Instance;
        var expl_poss = em.positions;
        var expl_rem_times = em.remaining_times;
        var expl_duration    = em.ExplosionDuration;

        for (var proj_i = 0; proj_i < proj_count;)
        {
            /* projectiles's shooter    */ var shooter = proj_shooters[proj_i];
            /* projectile's position    */ var proj_pos_3d = proj_poss[proj_i];
            /* projectile's direction   */ var proj_dir_3d = proj_dirs[proj_i];
            /* projectile's frame speed */ var proj_speed = proj_speeds[proj_i] * dt;
            /* next projectile point    */ var proj_next_pos = proj_pos_3d + proj_dir_3d * proj_speed;
            /* hit position             */ var hit_pos = proj_next_pos.xy();
            
            var hit = check_cell(proj_pos_3d) || check_cell(proj_next_pos);
            
            bool check_cell(Vector2 p)
            {
                /* cell's index       */ var cell_i  = grid.get_index_of(p);
                /* cell's positions   */ var cupos = unit_poss[cell_i];
                /* cell's units       */ var cunits= unit_refs[cell_i];
                /* cell's units count */ var cucount = cupos.Count;
                for (var unit_i = 0; unit_i < cucount; unit_i++)
                {
                    var unit = cunits[unit_i];
                    if (ReferenceEquals(unit, shooter))
                        continue;

                    /* unit's position 2d */ var upos2 = cupos[unit_i];
                    /* unit's position 3d */ var upos3 = upos2.xy(map.z(upos2) + hit_radius);

                    if (rays.try_intersect_sphere(proj_pos_3d, proj_dir_3d, proj_speed, upos3, hit_radius, out var t))
                    {
                        unit.incoming_damages.Add(proj_damages[proj_i]);
                        hit_pos = proj_pos_3d + proj_dir_3d * t;
                        return true;
                    }
                }

                return false;
            }

            // check collision with the terrain
            if (!hit)
            {
                /* projectiles int position      */ var cpc = map.coord_of(proj_pos_3d.xy());
                /* projectiles next int position */ var npc = map.coord_of(proj_next_pos.xy());

                var x0 = cpc.x;
                var y0 = cpc.y;
                var x1 = npc.x;
                var y1 = npc.y;

                var sdx = x1 - x0;
                var sx = sdx > 0 ? 1 : -1;
                var dx = sx * sdx;

                var sdy = y0 - y1;
                var sy = sdy < 0 ? 1 : -1;
                var dy = sy * sdy;

                var e = dx + dy;  /* error value e_xy */
                var first = true;

                while (true)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else 
                    {
                        if (proj_next_pos.z <= map.z(x1, y1)) //TODO?: calculate z by interpolation
                        {
                            /* projectile's position 2d */ var proj_pos_2d  = proj_pos_3d.xy();
                            /* projectile's position 2d */ var proj_dir_2d  = proj_dir_3d.xy();
                            /* cell's center            */ var cell_center = m2w.apply_to_point(x1, y1);
                            /* delta to cell's center   */ var delta   = cell_center - proj_pos_2d;
                            /* projection of delta onto direction */ var dproj = Vector2.Dot(proj_dir_2d, delta);
                            hit = true;
                            hit_pos = proj_pos_2d + proj_dir_2d * dproj;
                            break;
                        }
                    }

                    if (x0 == x1 && y0 == y1) break;

                    var e2 = 2 * e;
                    if (e2 >= dy) 
                    {
                        e += dy; /* e_xy+e_x > 0 */
                        x0 += sx;
                    }
                    if (e2 <= dx) /* e_xy+e_y < 0 */
                    {
                        e += dx;
                        y0 += sy;
                    }
                }
            }

            if (!hit && proj_next_pos.xy().sqrMagnitude > bound_radius_sqr)
            {
                hit = true;
            }

            if (hit)
            {
                expl_poss.Add(hit_pos);
                expl_rem_times.Add(expl_duration);

                // remove projectile
                proj_shooters.ReplaceWithLast(proj_i);
                proj_poss.ReplaceWithLast(proj_i);
                proj_prev_poss.ReplaceWithLast(proj_i);
                proj_dirs.ReplaceWithLast(proj_i);
                proj_damages.ReplaceWithLast(proj_i);
                proj_speeds.ReplaceWithLast(proj_i);
                proj_count--;
            }
            else
            {
                // update position
                proj_prev_poss[proj_i] = proj_poss[proj_i].xy();
                proj_poss [proj_i] = proj_next_pos;
                proj_i++;
            }
        }
    }
}