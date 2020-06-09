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
        /* speeds         */ var spds  = pm.speeds;
        /* damages        */ var dmgs  = pm.damages;
        /* count          */ var count = poss.Count;
        /* hit radius */ var hradius = pm.HitRadius;

        /* delta time            */ var dt     = Game.Instance.DeltaTime;
        /* map                   */ var map    = Map.Instance;
        /* map 2 world transform */ var m2w    = map.map_to_world;
        /* units registry        */ var ur     = UnitsRegistry.Instance;
        /* units' space grids    */ var grid   = ur.all_units_grid;
        /* grid's unit postions  */ var uposs  = grid.unit_positions;
        /* grid's units          */ var units  = grid.unit_refs;
        /* bounding radius ^2    */ var bradius2 = map.BoundingRadius.sqr();

        var em = ExplosionsManager.Instance;
        var eposs   = em.positions;
        var ertimes = em.remaining_times;
        var edur    = em.ExplosionDuration;

        for (int iproj = 0; iproj < count;)
        {
            /* projectiles's shooter    */ var shooter = pshooters[iproj];
            /* projectile's position    */ var ppos3d  = poss[iproj];
            /* projectile's direction   */ var pdir3d  = dirs[iproj];
            /* projectile's frame speed */ var spd  = spds[iproj] * dt;
            /* next projectile point    */ var npos = ppos3d + pdir3d * spd;
            /* hit position             */ var hit_pos = npos.xy();
            
            var hit = check_cell(ppos3d) || check_cell(npos);
            
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

                    if (rays.try_intersect_sphere(ppos3d, pdir3d, spd, upos3, hradius, out var t))
                    {
                        unit.incoming_damages.Add(dmgs[iproj]);
                        hit_pos = ppos3d + pdir3d * t;
                        return true;
                    }
                }

                return false;
            }

            // check collision with the terrain
            if (!hit)
            {
                /* projectiles int position      */ var cpc = map.coord_of(ppos3d.xy());
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
                            /* projectile's position 2d */ var ppos2d  = ppos3d.xy();
                            /* projectile's position 2d */ var pdir2d  = pdir3d.xy();
                            /* cell's center            */ var ccenter = m2w.apply_to_point(x1, y1);
                            /* delta to cell's center   */ var delta   = ccenter - ppos2d;
                            /* projection of delta onto direction */ var dproj = Vector2.Dot(pdir2d, delta);
                            hit = true;
                            hit_pos = ppos2d + pdir2d * dproj;
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

            if (!hit && npos.xy().sqrMagnitude > bradius2)
            {
                hit = true;
            }

            if (hit)
            {
                eposs.Add(hit_pos);
                ertimes.Add(edur);

                // remove projectile
                pshooters.ReplaceWithLast(iproj);
                poss .ReplaceWithLast(iproj);
                pposs.ReplaceWithLast(iproj);
                dirs .ReplaceWithLast(iproj);
                dmgs .ReplaceWithLast(iproj);
                spds .ReplaceWithLast(iproj);
                count--;
            }
            else
            {
                // update position
                pposs[iproj] = poss[iproj].xy();
                poss [iproj] = npos;
                iproj++;
            }
        }
    }
}