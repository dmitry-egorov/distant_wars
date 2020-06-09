using System;
using Plugins.Lanski;
using UnityEngine;

internal class update_unit_visibility : IMassiveMechanic
{
    public void _()
    {
        var lp = LocalPlayer.Instance;
        var lp_team_mask = lp.Faction.Team.Mask;

        var ur = UnitsRegistry.Instance;

        /* units' grids            */ var grid   = ur.all_units_grid;
        /* grid unit positions     */ var guposs = grid.unit_positions;
        /* grid unit detections    */ var gudets = grid.unit_detections_by_team;
        /* grid unit discoveries   */ var gudiss = grid.unit_identifications_by_team;

        /* grid's fully radar visible cells */ var gcfdets = grid.cell_full_detections_by_team;
        Array.Clear(gcfdets, 0, gcfdets.Length);
        /* grid's fully visible cells */ var gcfvis = grid.cell_full_visibilities_by_team;
        Array.Clear(gcfvis, 0, gcfvis.Length);

        /* cell's radius  */ var cell_radius = grid.cell_radius;
        /* cell's centers */ var ccenters = grid.cell_centers;

        /* all units      */ var units  = ur.all_units;
        /* all unit count */ var ucount = units.Count;

        //var total_cells = 0;
        //var full_cells = 0;
        //var partial_cells = 0;
        //var skipped_cells_fully_visible = 0;
        //var skipped_cells_outside = 0;

        for (int unit_i = 0; unit_i < ucount; unit_i++)
        {
            /* the unit         */ var unit = units[unit_i];
            /* unit's position  */ var upos = unit.position;
            /* unit's team mask */ var uteam_mask = unit.Faction.Team.Mask;

            /* unit's vision range     */ var uvis  = unit.VisionRange;
            /* unit's vision range ^2  */ var uvis2 = uvis.sqr();
            /* own radar range         */ var uradar  = uvis + unit.RadarRangeExtension;
            /* own radar range ^2      */ var uradar2 = uradar.sqr();
            /* radar + cell radius ^2  */ var uradar_p_crad2 = (uradar + cell_radius).sqr();
            /* radar - cell radius     */ var uradar_m_crad  = uradar - cell_radius;
            /* radar - cell radius ^2  */ var uradar_m_crad2 = uradar_m_crad >= 0 ? uradar_m_crad.sqr() : -1;
            /* radar + cell radius ^2  */ var uvis_p_crad2 = (uvis + cell_radius).sqr();
            /* radar - cell radius     */ var uvis_m_crad  = uvis - cell_radius;
            /* radar - cell radius ^2  */ var uvis_m_crad2 = uvis_m_crad >= 0 ? uvis_m_crad.sqr() : -1;

            var it = grid.get_iterator_of_circle(upos, uradar);
            while (it.next(out var cell_i))
            {
                if (cell_i == 0)
                // is the outer cell
                {
                    process_partially_radar_visible_cell();
                    process_partially_visible_cell();
                    continue;
                }
                
                if ((gcfvis[cell_i] & uteam_mask) > 0)
                // cell was fully visible before
                    continue;

                /* cell's center */ var ccenter = ccenters[cell_i];
                /* delta from unit to cell's center       */ var ucdelta = ccenter - upos;
                /* distance ^2 from unit to cell's center */ var ucdist2 = ucdelta.sqrMagnitude;

                if ((gcfdets[cell_i] & uteam_mask) > 0)
                // cell was fully radar visible before
                {
                    // is not marked as fully visible, so we check its visibility
                    check_vision_of_radar_visible_cell();
                    continue;
                }

                if (ucdist2 > uradar_p_crad2)
                // cell is outside the unit's radar vision (and therefore normal vision)
                    continue;

                if (ucdist2 > uradar_m_crad2)
                // cell is partially radar visible
                {
                    process_partially_radar_visible_cell();

                    if (ucdist2 > uvis_p_crad2)
                    // cell is outside the vision
                        continue;

                    // cell is partially visible
                    // cell is only partially visible by the radar, so it can't be fully visible
                    process_partially_visible_cell();
                }
                // cell is compeletely inside the radar area
                else
                {
                    process_fully_radar_visiible_cell();
                    check_vision_of_radar_visible_cell();    
                }

                void check_vision_of_radar_visible_cell()
                {
                    if (ucdist2 > uvis_p_crad2)
                        // cell is outside the vision
                        return;

                    if (ucdist2 > uvis_m_crad2)
                        process_partially_visible_cell();
                    else
                        process_fully_visiible_cell();
                }


                void process_fully_radar_visiible_cell()
                {
                    /* cell units visibilities */ var cudets  = gudets[cell_i];
                    /* cell units count        */ var cucount = cudets.Count;
                    for (var ui = 0; ui < cucount; ui++)
                    {
                        cudets[ui] = (byte)(cudets[ui] | uteam_mask); // mark unit as visible
                    }

                    gcfdets[cell_i] = (byte)(gcfdets[cell_i] | uteam_mask); // mark cell as radar visible
                }

                void process_fully_visiible_cell()
                {
                    /* cell units discoveries */ var cudiss  = gudiss[cell_i];
                    /* cell units count       */ var cucount = cudiss.Count;
                    for (var ui = 0; ui < cucount; ui++)
                    {
                        /* other unit's discovery */ ref var ou_dis = ref cudiss[ui];
                        if ((ou_dis & uteam_mask) > 0) // other unit is already discovered 
                            continue;

                        ou_dis = (byte)(ou_dis | uteam_mask); // mark unit as discovered
                    }

                    gcfvis[cell_i] = (byte)(gcfvis[cell_i] | uteam_mask); // mark cell as visible
                }

                void process_partially_radar_visible_cell()
                {
                    /* cell positions    */ var cuposs  = guposs[cell_i];
                    /* cell visibilities */ var cudets  = gudets[cell_i];
                    /* cell units count  */ var cucount = cuposs.Count;

                    for (int ui = 0; ui < cucount; ui++)
                    {
                        /* other unit's visibility */ ref var ou_radar = ref cudets[ui];
                        if ((ou_radar & uteam_mask) > 0) // other unit is already marked as visible 
                            continue;

                        /* other unit's position */ var oup = cuposs[ui];
                        if ((oup - upos).sqrMagnitude > uradar2) // other unit is outside the range
                            continue;

                        ou_radar = (byte)(ou_radar | uteam_mask); // mark unit as visible
                    }
                }

                void process_partially_visible_cell()
                {
                    /* cell positions    */ var cuposs  = guposs[cell_i];
                    /* cell discoveries  */ var cudiss  = gudiss[cell_i];
                    /* cell units count  */ var cucount = cudiss.Count;

                    for (int ui = 0; ui < cucount; ui++)
                    {
                        /* other unit's discovery */ ref var ou_dis = ref cudiss[ui];
                        if ((ou_dis & uteam_mask) > 0) // other unit is already discovered 
                                continue;

                        /* other unit's position */ var oup = cuposs[ui];
                        if ((oup - upos).sqrMagnitude > uvis2) // other unit is outside the vision range
                            continue;

                        ou_dis = (byte)(ou_dis | uteam_mask); // mark unit as discovered
                    }
                }
            }

            Debug.Assert(gcfdets[0] == 0); // sanity check: external cell is not fully radar visible
            Debug.Assert(gcfvis[0] == 0); // sanity check: external cell is not fully visible
        }

        //DebugText.set_text("total cells",   total_cells.ToString());
        //DebugText.set_text("partial cells", partial_cells.ToString());
        //DebugText.set_text("full cells",    full_cells.ToString());
        //DebugText.set_text("skipped cells visited", skipped_cells_fully_visible.ToString());
        //DebugText.set_text("skipped cells outside", skipped_cells_outside.ToString());
    }
}