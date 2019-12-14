using System;
using Plugins.Lanski;
using UnityEngine;

internal class update_unit_visibility : MassiveMechanic
{
    public void _()
    {
        var lp = LocalPlayer.Instance;
        var lp_team_mask = lp.Faction.Team.Mask;

        var ur = UnitsRegistry.Instance;

        /* units' grids            */ var grid   = ur.SpaceGrid;
        /* grid unit positions     */ var guposs = grid.unit_positions;
        /* grid unit visibilities  */ var guviss = grid.unit_visibilities;

        /* grid's fully visible cells */ var gcfviss = grid.cell_full_visibilities;
        Array.Clear(gcfviss, 0, gcfviss.Length);

        /* cell's radius  */ var cell_radius  = grid.cell_radius;
        /* cell's centers */ var ccenters = grid.cell_centers;

        /* all units      */ var units  = ur.Units;
        /* all unit count */ var ucount = units.Count;

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
            /* vision - cell radius    */ var uvis_m_crad  = uvis - cell_radius;
            /* vision - cell radius ^2 */ var uvis_m_crad2 = uvis_m_crad >= 0 ? uvis_m_crad.sqr() : -1;

            var it = grid.get_iterator_of_circle(upos, uvis);
            while (it.next(out var cell_i))
            {
                if (cell_i != 0) // is not in the outer cell
                {
                    // cell was already fully visible
                    if ((gcfviss[cell_i] & uteam_mask) > 0)
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
                    
                    // cell is compeletely inside the vision area
                    if (ucdist2 <= uvis_m_crad2)
                    {
                        //full_cells++;

                        /* cell units visibilities */ var cuviss  = guviss[cell_i];
                        /* cell units count        */ var cucount = cuviss.Count;
                        for (var ui = 0; ui < cucount; ui++)
                        {
                            cuviss[ui] = (byte)(cuviss[ui] | uteam_mask); // mark unit as visible
                        }

                        gcfviss[cell_i] = (byte)(gcfviss[cell_i] | uteam_mask); // mark cell as visible
                        continue;
                    }
                }

                // cell is partially inside the vision area
                {
                    //partial_cells++;

                    /* cell positions    */ var cuposs  = guposs[cell_i];
                    /* cell visibilities */ var cuviss  = guviss[cell_i];
                    /* cell units count  */ var cucount = cuposs.Count;

                    for (int ui = 0; ui < cucount; ui++)
                    {
                        /* other unit's visibility */ ref var ou_vis = ref cuviss[ui];
                        if ((ou_vis & uteam_mask) > 0) // other unit is already marked as visible 
                                continue;

                        /* other unit's position */ var oup = cuposs[ui];
                        if ((oup - upos).sqrMagnitude > uvis2) // other unit is outside the range
                            continue;

                        ou_vis = (byte)(ou_vis | uteam_mask); // mark unit as visible
                    }
                }
            }

            Debug.Assert(gcfviss[0] == 0); // sanity check: external cell is not fully visible
        }

        //DebugText.set_text("total cells",   total_cells.ToString());
        //DebugText.set_text("partial cells", partial_cells.ToString());
        //DebugText.set_text("full cells",    full_cells.ToString());
        //DebugText.set_text("skipped cells visited", skipped_cells_fully_visible.ToString());
        //DebugText.set_text("skipped cells outside", skipped_cells_outside.ToString());
    }
}