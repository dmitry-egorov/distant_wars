using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class cleanup_dead_units : IMassiveMechanic
{
    public void _() 
    {
        var ur = UnitsRegistry.Instance;
        var us = ur.all_units;
        var own_units = ur.local_team_units;

        /* space grid */ var grid  = ur.all_units_grid;
        var grid_unit_poss  = grid.unit_positions;
        var grid_unit_prev_poss = grid.unit_prev_positions;
        var grid_unit_teams = grid.unit_teams;
        var grid_unit_refs  = grid.unit_refs;
        var grid_unit_detects  = grid.unit_detections_by_team;
        var grid_unit_idents  = grid.unit_identifications_by_team;

        for (var i = 0; i < us.Count; i++)
        {
            var u = us[i];
            if (u.hit_points > 0)
                continue;
            
            // remove from units
            us.ReplaceWithLast(i);

            var (cell_i, unit_i) = u.space_grid_index;

            UnitsSpaceGrid2.remove_unit_from_cell
            (
                unit_i
                , grid_unit_refs[cell_i]
                ,grid_unit_poss[cell_i]
                , grid_unit_prev_poss[cell_i]
                , grid_unit_teams[cell_i]
                , grid_unit_detects[cell_i]
                , grid_unit_idents[cell_i]
            );

            // remove from own units
            {
                var own_units_i = u.own_units_index;
                if (own_units_i != -1)
                {
                    own_units.ReplaceWithLast(own_units_i);
                    if (own_units_i < own_units.Count) own_units[own_units_i].own_units_index = own_units_i;
                }
            }
                
            // remove from unity
            Object.Destroy(u.gameObject);
        }
    }
}