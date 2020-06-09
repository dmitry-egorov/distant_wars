using Plugins.Lanski;
using UnityEngine;

public class cleanup_dead_units : MassiveMechanic
{
    public void _() 
    {
        var ur = UnitsRegistry.Instance;
        var us = ur.Units;
        var ous = ur.OwnTeamUnits;

        /* space grid */ var sg  = ur.SpaceGrid;
        /* grid positions      */ var guposs  = sg.unit_positions;
        /* grid prev positions */ var gupposs = sg.unit_prev_positions;
        /* grid team ids       */ var guteams = sg.unit_teams;
        /* grid units          */ var gunits  = sg.unit_refs;
        /* grid visiblities    */ var guviss  = sg.unit_detections_by_team;
        /* grid visiblities    */ var gudiss  = sg.unit_indentifications_by_team;

        for (var i = 0; i < us.Count; i++)
        {
            var u = us[i];
            if (u.hit_points > 0) 
                continue;
            
            // remove from units
            us.ReplaceWithLast(i);

            // delete from space grid
            {
                var (icell, icunit) = u.space_grid_index;
                var cunits = gunits[icell];
                cunits.ReplaceWithLast(icunit);
                guposs [icell].ReplaceWithLast(icunit);
                gupposs[icell].ReplaceWithLast(icunit);
                guteams[icell].ReplaceWithLast(icunit);
                guviss [icell].ReplaceWithLast(icunit);
                gudiss [icell].ReplaceWithLast(icunit);

                // fix the id of the replacing unit
                if (icunit < cunits.Count) cunits[icunit].space_grid_index.index = icunit;
            }
                
            // remove from own units
            {
                var otuindex = u.own_units_index;
                if (otuindex != -1)
                {
                    ous.ReplaceWithLast(otuindex);
                    if (otuindex < ous.Count) ous[otuindex].own_units_index = otuindex;
                }
            }
                
            // remove from unity
            Object.Destroy(u.gameObject);
        }
    }
}