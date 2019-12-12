using System;
using Plugins.Lanski;

// find visible units other than the player's
internal class update_unit_visibility : MassiveMechanic
{
    public void _()
    {
        var lp = LocalPlayer.Instance;
        var lp_team_mask = lp.Faction.Team.Mask;

        var ur = UnitsRegistry.Instance;

        /* unit's grids */ var usg  = ur.SpaceGrid;
        /* grid units   */ var sgu  = usg.unit_refs;
        /* grid unit positions    */ var gposs   = usg.unit_positions;
        /* grid unit visibilities */ var gviss   = usg.unit_visibilities;
        /* grids' cell count      */ var gccount = sgu.Length;

        /* fully visible cells */ var c_fviss = usg.cell_full_visibilities;
        Array.Clear(c_fviss, 0, c_fviss.Length);

        /* cell's radius  */ var cell_radius  = usg.cell_radius;
        /* cell's centers */ var ccenters = usg.cell_centers;

        var units = ur.Units;
        var ucount = units.Count;

        //var total_cells = 0;
        //var full_cells = 0;
        //var partial_cells = 0;
        //var skipped_cells_fully_visible = 0;
        //var skipped_cells_outside = 0;

        for (int unit_i = 0; unit_i < ucount; unit_i++)
        {
            /* the unit         */ var unit = units[unit_i];
            /* unit's position  */ var upos = unit.Position;
            /* unit's team mask */ var uteam_mask = unit.Faction.Team.Mask;
            /* own vision range        */ var uvis  = unit.VisionRange;
            /* own vision range ^2     */ var uvis2 = uvis.sqr();
            /* vision + cell radius ^2 */ var uvis_p_crad2 = (uvis + cell_radius).sqr();
            /* vision - cell radius ^2 */ var uvis_m_crad2 = (uvis - cell_radius).sqr();//Case, when vision radius is less than cell's?
            /* unit's grid vision area */ var uvis_area = usg.get_coord_rect_of_circle(upos, uvis);

            var minx = uvis_area.min.x;
            var miny = uvis_area.min.y;
            var maxx = uvis_area.max.x;
            var maxy = uvis_area.max.y;
            
            for (var yi = miny; yi <= maxy; yi++)
            for (var xi = minx; xi <= maxx; xi++)
            {
                //total_cells++;

                /* the cell */ var cell_i = usg.get_index_of(xi, yi);

                if (cell_i != 0) // is not in the outer cell
                {
                    // cell was already fully visible
                    if ((c_fviss[cell_i] & uteam_mask) > 0)
                    {
                        //skipped_cells_fully_visible++;
                        continue;
                    }
                    
                    /* cell's center */ var ccenter = ccenters[cell_i];
                    /* delta from unit to cell's center       */ var ucdelta = ccenter - upos;
                    /* distance ^2 from unit to cell's center */ var ucdist2 = ucdelta.sqrMagnitude;

                    // cell is outside the vision area
                    if (ucdist2 > uvis_p_crad2)
                    {
                        //skipped_cells_outside++;
                        continue;
                    }
                    else
                    // cell is compeletely inside the vision area
                    if (ucdist2 <= uvis_m_crad2)
                    {
                        //full_cells++;

                        /* cell units visibilities */ var cuviss  = gviss[cell_i];
                        /* cell units count        */ var cucount = cuviss.Count;
                        for (int i = 0; i < cucount; i++)
                        {
                            cuviss[i] = (byte)(cuviss[i] | uteam_mask); // mark unit as visible
                        }

                        c_fviss[cell_i] = (byte)(c_fviss[cell_i] | uteam_mask); // mark cell as visible
                        continue;
                    }
                }

                // cell is partially inside the vision area
                {
                    //partial_cells++;

                    /* cell positions    */ var cuposs  = gposs[cell_i];
                    /* cell visibilities */ var cuviss  = gviss[cell_i];
                    /* cell units count  */ var cucount = cuposs.Count;

                    for (int i = 0; i < cucount; i++)
                    {
                        ref var ou_vis = ref cuviss[i];
                        if ((ou_vis & uteam_mask) > 0) // other unit is already marked as visible 
                                continue;

                        /* other unit's position */ var oup = cuposs[i];
                        if ((oup - upos).sqrMagnitude > uvis2) // other unit is outside the range
                            continue;

                        ou_vis = (byte)(ou_vis | uteam_mask); // mark unit as visible
                    }
                }
            }
        }

        //DebugText.set_text("total cells",   total_cells.ToString());
        //DebugText.set_text("partial cells", partial_cells.ToString());
        //DebugText.set_text("full cells",    full_cells.ToString());
        //DebugText.set_text("skipped cells visited", skipped_cells_fully_visible.ToString());
        //DebugText.set_text("skipped cells outside", skipped_cells_outside.ToString());
    }
}