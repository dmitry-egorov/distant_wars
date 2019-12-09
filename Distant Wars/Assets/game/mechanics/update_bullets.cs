using Plugins.Lanski;
using UnityEngine;

internal class update_bullets : MassiveMechanic
{
    public void _()
    {
        var  pm = ProjectilesManager.Instance;

        var pps = pm.Positions;
        var drs = pm.Directions;
        var sps = pm.Speeds;
        var dms = pm.Damages;

        // update position
        /* delta time     */ var dt  = Time.deltaTime;
        /* map            */ var  m  = Map.Instance;
        /* units registry */ var ur  = UnitsRegistry.Instance;
        /* units          */ var us  = ur.Units;
        /* units          */ var usg = ur.SpaceGrid;
        /* count          */ var   c = pps.Count;

        for (int i = 0; i < c;)
        {
            /* projectile's position  */ var pp = pps[i];
            /* projectile's direction */ var dr = drs[i];
            /* projectile's offset    */ var po = sps[i] * dt;
            
            var hit = false;

            // check collision with units
            {
                var cell = usg.get_cell_of(pp);
                var ps = cell.positions;
                var gus = cell.elements;
                for(var j = 0; j < ps.Count; j++)
                {
                    /* unit's position 2d */ var up2 = ps[j];
                    /* unit's position 3d */ var up3 = m.xyz(up2);
                    /* unit               */ var   u = gus[j];
                    /* hit radius         */ var  hr = u.HitRadius;

                    hit = rays.try_intersect_sphere(pp, dr, up3, hr, out var t) && t >= 0 && t < po;
                    if (hit)
                    {
                        u.HitPoints -= dms[i];
                        break;
                    }
                } 
            }

            //check collision with the terrain
            if (!hit)
            {
                /* next projectile point         */ var npp = (pp + dr * po);
                /* projectiles int position      */ var cpc = m.coord_of(pp.xy());
                /* projectiles next int position */ var npc = m.coord_of(npp.xy());

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
                        first = false;
                    else if (hit = npp.z <= m.z(x0, y0)) //TODO: calculate z by interpolation?
                        break;

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
                pps.replace_with_last(i);
                drs.replace_with_last(i);
                dms.replace_with_last(i);
                sps.replace_with_last(i);
                c--;
            }
            else
            {
                // update position
                pps[i] = pp + dr * po;
                i++;
            }
        }
    }
}