using Plugins.Lanski;

internal class update_space_grid : MassiveMechanic
{
    public void _()
    {
        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* local player     */ var lp  = LocalPlayer.Instance;

        /* space grid */ var sg  = ur.SpaceGrid;
        /* grid positions      */ var guposs  = sg.unit_positions;
        /* grid prev positions */ var gupposs = sg.unit_prev_positions;
        /* grid team ids       */ var guteams = sg.unit_team_masks;
        /* grid units          */ var gunits  = sg.unit_refs;
        /* grid visiblities    */ var guviss  = sg.unit_detections;
        /* grid visiblities    */ var gudiss  = sg.unit_discoveries;
        /* grid cell count     */ var ccount  = guposs.Length;

        for (var ci = 0; ci < ccount; ci++)
        {
            /* cell's unit positions      */ var cposs  = guposs [ci];
            /* cell's unit prev positions */ var cpposs = gupposs[ci];
            /* cell's unit team ids       */ var cteams = guteams[ci];
            /* cell's units               */ var cunits = gunits [ci];
            /* cell's units visibility    */ var cviss  = guviss [ci];
            /* cell's units dicovery      */ var cdiss  = gudiss [ci];
            /* unit count                 */ var ucount = cposs.Count;

            for (int ui = 0; ui < ucount;)
            {
                /* unit's team mask */ var tid  = cteams[ui];

                /* the unit */ var unit = cunits[ui];
                /* unit's new position   */ var npos = unit.Position;
                /* unit's prev position  */ var ppos = unit.PrevPosition;
                /* unit's new cell index */ var new_ci = sg.get_index_of(npos);

                if (new_ci == ci)
                // is in the old cell
                {
                    cpposs[ui] = ppos;
                    cposs [ui] = npos;
                    cviss [ui] = tid; // reset visibility to the unit's own team only
                    ui++;

                    continue;
                }

                // is in a new cell
                // add to the new cell
                {
                    /* new cell's units */ var ncunits = gunits[new_ci];
                    unit.SpaceGridIndex = (new_ci, ncunits.Count);
                    ncunits.Add(unit);

                    guposs [new_ci].Add(npos);
                    gupposs[new_ci].Add(ppos);
                    guteams[new_ci].Add(tid);
                    guviss [new_ci].Add(tid); // reset visibility to the unit's own team only
                    gudiss [new_ci].Add(cdiss[ui]);
                }

                // remove from the old cell
                {
                    cunits.ReplaceWithLast(ui);
                    cposs .ReplaceWithLast(ui);
                    cpposs.ReplaceWithLast(ui);
                    cteams.ReplaceWithLast(ui);
                    cviss .ReplaceWithLast(ui);
                    cdiss .ReplaceWithLast(ui);

                    // fix the id of the replacing unit
                    if (ui < cunits.Count) cunits[ui].SpaceGridIndex.index = ui;
                }

                ucount--;
            }
        }
    }
}