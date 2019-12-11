using Plugins.Lanski;

internal class update_space_grid : MassiveMechanic
{
    public void _()
    {
        /* units' registry */ var ur = UnitsRegistry.Instance;
        /* local player    */ var lp = LocalPlayer.Instance;
        /* space grid             */ var sg = ur.SpaceGrid;
        /* grid positions         */ var gps  = sg.cells_positions;
        /* grid team ids          */ var gts  = sg.cells_team_ids;
        /* grid units             */ var gus  = sg.cells_units;
        /* grid visiblities       */ var gvs  = sg.cells_visibilities;
        /* grid full visibilities */ var gfvs = sg.cell_full_visibility;
        /* grid cell count        */ var ccount = gps.Length;

        var ptid = lp.Faction.Team.Index;

        for (var ci = 0; ci < ccount; ci++)
        {
            gfvs[ci] = false; // reset full visibility

            /* cell's unit positions   */ var cps  = gps[ci];
            /* cell's unit team ids    */ var cts  = gts[ci];
            /* cell's units            */ var cus  = gus[ci];
            /* cell's units visibility */ var cuvs = gvs[ci];
            /* unit count              */ var ucount = cps.Count;

            for (int ui = 0; ui < ucount;)
            {
                /* unit's team id */ var tid  = cts[ui];
                if (tid != ptid) // unit doesn't belong to the player's team
                {
                    cuvs[ui] = false;
                }

                /* the unit              */ var unit = cus[ui];
                /* new unit's position   */ var np   = unit.Position;
                /* unit's new cell index */ var nci  = sg.get_index_of(np);

                if (nci != ci)
                {
                    gps[nci].Add(np);
                    gts[nci].Add(tid);
                    gus[nci].Add(unit);
                    gvs[nci].Add(cuvs[ui]);

                    cps .ReplaceWithLast(ui);
                    cts .ReplaceWithLast(ui);
                    cus .ReplaceWithLast(ui);
                    cuvs.ReplaceWithLast(ui);

                    ucount--;
                }
                else
                {
                    cps[ui] = np;
                    ui++;
                }
            }
        }
    }
}