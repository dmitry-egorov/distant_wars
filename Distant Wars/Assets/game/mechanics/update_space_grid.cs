using Plugins.Lanski;

internal class update_space_grid : IMassiveMechanic
{
    public void _()
    {
        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* local player     */ var lp  = LocalPlayer.Instance;

        /* space grid */ var sg  = ur.all_units_grid;
        /* grid positions      */ var guposs  = sg.unit_positions;
        /* grid prev positions */ var gupposs = sg.unit_prev_positions;
        /* grid team ids       */ var guteams = sg.unit_teams;
        /* grid units          */ var gunits  = sg.unit_refs;
        /* grid visiblities    */ var guviss  = sg.unit_detections_by_team;
        /* grid visiblities    */ var gudiss  = sg.unit_identifications_by_team;
        /* grid cell count     */ var ccount  = guposs.Length;

        for (var cell_i = 0; cell_i < ccount; cell_i++)
        {
            /* cell's unit positions      */ var cposs  = guposs [cell_i];
            /* cell's unit prev positions */ var cpposs = gupposs[cell_i];
            /* cell's unit team ids       */ var cteams = guteams[cell_i];
            /* cell's units               */ var cunits = gunits [cell_i];
            /* cell's units visibility    */ var cviss  = guviss [cell_i];
            /* cell's units dicovery      */ var cdiss  = gudiss [cell_i];
            /* unit count                 */ var ucount = cposs.Count;

            for (var unit_i = 0; unit_i < ucount;)
            {
                /* unit's team mask */ var tid  = cteams[unit_i];

                /* the unit */ var unit = cunits[unit_i];
                /* unit's new position   */ var new_pos  = unit.position;
                /* unit's prev position  */ var prev_pos = unit.prev_position;
                /* unit's new cell index */ var new_ci = sg.get_index_of(new_pos);

                if (new_ci == cell_i)
                // is in the old cell
                {
                    cpposs[unit_i] = prev_pos;
                    cposs [unit_i] = new_pos;
                    cviss [unit_i] = tid; // reset visibility to the unit's own team only
                    unit_i++;

                    continue;
                }

                // is in a new cell
                // add to the new cell
                {
                    /* new cell's units */ var ncunits = gunits[new_ci];
                    unit.space_grid_index = (new_ci, ncunits.Count);
                    ncunits.Add(unit);

                    guposs [new_ci].Add(new_pos);
                    gupposs[new_ci].Add(prev_pos);
                    guteams[new_ci].Add(tid);
                    guviss [new_ci].Add(tid); // reset visibility to the unit's own team only
                    gudiss [new_ci].Add(cdiss[unit_i]);
                }

                // remove from the old cell
                UnitsSpaceGrid2.remove_unit_from_cell
                (
                    unit_i
                    , cunits
                    , cposs
                    , cpposs
                    , cteams
                    , cviss
                    , cdiss
                );
                
                ucount--;
            }
        }
    }
}