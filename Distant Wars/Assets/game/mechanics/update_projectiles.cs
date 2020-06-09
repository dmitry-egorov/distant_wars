using Plugins.Lanski;
using UnityEngine;

internal class update_projectiles : IMassiveMechanic
{
    public void _()
    {
        var pm  = ProjectilesManager.Instance;
        /* shooters */ var pshooters = pm.shooters;
        /* positions      */ var poss  = pm.positions;
        /* prev positions */ var pposs = pm.prev_positions;
        /* directions     */ var dirs  = pm.directions;
        /* speeds         */ var speeds  = pm.speeds;
        /* damages        */ var dmgs  = pm.damages;
        /* count          */ var count = poss.Count;
        /* hit radius */ var hradius = pm.HitRadius;

        /* delta time            */ var dt = Game.Instance.DeltaTime;
        /* map                   */ var map = Map.Instance;
        /* map 2 world transform */ var m2w = map.map_to_world;
        /* units registry        */ var ur = UnitsRegistry.Instance;
        /* units' space grids    */ var grid = ur.all_units_grid;
        /* grid's unit positions */ var uposs  = grid.unit_positions;
        /* grid's units          */ var units  = grid.unit_refs;
        /* bounding radius ^2    */ var bradius_sqr = map.BoundingRadius.sqr();

        var em = ExplosionsManager.Instance;
        var expl_poss = em.positions;
        var expl_rem_times = em.remaining_times;
        var expl_duration    = em.ExplosionDuration;

        for (var proj_i = 0; proj_i < count;)
        {
            /* projectiles's shooter    */ var shooter = pshooters[proj_i];
            /* projectile's position    */ var proj_pos_3d = poss[proj_i];
            /* projectile's direction   */ var proj_dir_3d = dirs[proj_i];
            /* projectile's frame speed */ var proj_speed = speeds[proj_i] * dt;
            /* next projectile point    */ var npos = proj_pos_3d + proj_dir_3d * proj_speed;
            /* hit position             */ var hit_pos = npos.xy();
            
            var hit = check_cell(proj_pos_3d) || check_cell(npos);
            
            bool check_cell(Vector2 p)
            {
                /* cell's index       */ var cell_i  = grid.get_index_of(p);
                /* cell's positions   */ var cupos   = uposs[cell_i];
                /* cell's units       */ var cunits  = units[cell_i];
                /* cell's units count */ var cucount = cupos.Count;
                for (var unit_i = 0; unit_i < cucount; unit_i++)
                {
                    var unit = cunits[unit_i];
                    if (ReferenceEquals(unit, shooter))
                        continue;

                    /* unit's position 2d */ var upos2 = cupos[unit_i];
                    /* unit's position 3d */ var upos3 = upos2.xy(map.z(upos2) + hradius);

                    if (rays.try_intersect_sphere(proj_pos_3d, proj_dir_3d, proj_speed, upos3, hradius, out var t))
                    {
                        unit.incoming_damages.Add(dmgs[proj_i]);
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
                /* projectiles next int position */ var npc = map.coord_of(npos.xy());

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
                        if (npos.z <= map.z(x1, y1)) //TODO?: calculate z by interpolation
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

            if (!hit && npos.xy().sqrMagnitude > bradius_sqr)
            {
                hit = true;
            }

            if (hit)
            {
                expl_poss.Add(hit_pos);
                expl_rem_times.Add(expl_duration);

                // remove projectile
                pshooters.ReplaceWithLast(proj_i);
                poss.ReplaceWithLast(proj_i);
                pposs.ReplaceWithLast(proj_i);
                dirs.ReplaceWithLast(proj_i);
                dmgs.ReplaceWithLast(proj_i);
                speeds.ReplaceWithLast(proj_i);
                count--;
            }
            else
            {
                // update position
                pposs[proj_i] = poss[proj_i].xy();
                poss [proj_i] = npos;
                proj_i++;
            }
        }
    }
}