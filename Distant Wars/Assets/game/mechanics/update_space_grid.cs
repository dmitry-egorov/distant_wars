using Plugins.Lanski;

internal class update_space_grid : MassiveMechanic
{
    public void _()
    {
        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* local player     */ var lp  = LocalPlayer.Instance;
        /* space grid       */ var sg  = ur.SpaceGrid;
        /* grid positions   */ var gps = sg.unit_positions;
        /* grid team ids    */ var gts = sg.unit_team_masks;
        /* grid units       */ var gus = sg.unit_refs;
        /* grid visiblities */ var gvs = sg.unit_visibilities;
        /* grid cell count  */ var ccount = gps.Length;

        var lp_team_mask = lp.Faction.Team.Mask;

        for (var ci = 0; ci < ccount; ci++)
        {
            /* cell's unit positions   */ var cps  = gps[ci];
            /* cell's unit team ids    */ var cts  = gts[ci];
            /* cell's units            */ var cus  = gus[ci];
            /* cell's units visibility */ var cuvs = gvs[ci];
            /* unit count              */ var ucount = cps.Count;

            for (int ui = 0; ui < ucount;)
            {
                /* unit's team mask */ var tid  = cts[ui];
                cuvs[ui] = tid; // reset visibility to the unit's own team only

                /* the unit */ var unit = cus[ui];

                if (unit == null)
                {
                    cps .ReplaceWithLast(ui);
                    cts .ReplaceWithLast(ui);
                    cus .ReplaceWithLast(ui);
                    cuvs.ReplaceWithLast(ui);

                    ucount--;
                    continue;
                }

                /* new unit's position   */ var npos = unit.Position;
                /* unit's new cell index */ var new_ci = sg.get_index_of(npos);

                if (new_ci != ci)
                {
                    gps[new_ci].Add(npos);
                    gts[new_ci].Add(tid);
                    gus[new_ci].Add(unit);
                    gvs[new_ci].Add(cuvs[ui]);

                    cps .ReplaceWithLast(ui);
                    cts .ReplaceWithLast(ui);
                    cus .ReplaceWithLast(ui);
                    cuvs.ReplaceWithLast(ui);

                    ucount--;
                    continue;
                }

                cps[ui] = npos;
                ui++;
            }
        }
    }
}