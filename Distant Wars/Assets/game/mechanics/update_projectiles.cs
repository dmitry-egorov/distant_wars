using Plugins.Lanski;

internal class update_projectiles : MassiveMechanic
{
    public void _()
    {
        var pm  = ProjectilesManager.Instance;
        /* positions  */ var poss  = pm.positions;
        /* positions  */ var pposs = pm.prev_positions;
        /* directions */ var dirs  = pm.directions;
        /* speeds     */ var spds  = pm.speeds;
        /* damages    */ var dmgs  = pm.damages;
        /* count      */ var count = poss.Count;
        /* hit radius */ var hradius  = pm.HitRadius;

        /* delta time           */ var dt     = Game.Instance.DeltaTime;
        /* map                  */ var map    = Map.Instance;
        /* units registry       */ var ur     = UnitsRegistry.Instance;
        /* units' space grids   */ var grid   = ur.SpaceGrid;
        /* grid's unit postions */ var uposs  = grid.unit_positions;
        /* grid's units         */ var units  = grid.unit_refs;

        for (int iproj = 0; iproj < count;)
        {
            /* projectile's position    */ var pos  = poss[iproj];
            /* projectile's direction   */ var dir  = dirs[iproj];
            /* projectile's frame speed */ var spd  = spds[iproj] * dt;
            /* next projectile point    */ var npos = pos + dir * spd;
            
            var hit = false;

            // check collision with units
            {
                //TODO?: check more than one cell
                /* cell's index       */ var cell_i  = grid.get_index_of(npos);
                /* cell's positions   */ var cupos   = uposs[cell_i];
                /* cell's units       */ var cunits  = units[cell_i];
                /* cell's units count */ var cucount = cupos.Count;
                for (var unit_i = 0; unit_i < cucount; unit_i++)
                {
                    /* unit's position 2d */ var upos2 = cupos[unit_i];
                    /* unit's position 3d */ var upos3 = map.xyz(upos2);

                    hit = rays.try_intersect_sphere(pos, dir, upos3, hradius, out var t) && t >= 0 && t < spd;
                    if (hit)
                    {
                        cunits[unit_i].IncomingDamages.Add(dmgs[iproj]);
                        break;
                    }
                }
            }

            // check collision with the terrain
            if (!hit)
            {
                /* projectiles int position      */ var cpc = map.coord_of(pos.xy());
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
                    else if (npos.z <= map.z(x1, y1)) //TODO?: calculate z by interpolation
                    {
                        hit = true;
                        break;
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

            if (hit)
            {
                // remove projectile
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