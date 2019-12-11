using System;
using Plugins.Lanski;

// find visible units other than the player's
internal class find_visible_units_from_other_teams : MassiveMechanic
{
    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var ovus = ur.OtherTeamsVisibleUnits;

        ovus.Clear();

        /* unit's space grids      */ var usg  = ur.SpaceGrid;
        /* space grid units        */ var sgu  = usg.cells_units;
        /* space grid units        */ var sgp  = usg.cells_positions;
        /* space grid units        */ var sgv  = usg.cells_visibilities;
        /* space grids' cell count */ var sgcc = sgu.Length;

        /* fully visible cells */ var fvc = usg.cell_full_visibility;
        Array.Clear(fvc, 0, fvc.Length);

        /* cell's radius  */ var cr  = usg.cell_radius;
        /* cell's centers */ var ccs = usg.cells_centers;
        
        foreach (/* own unit */ var owu in ur.OwnTeamUnits)
        {
            /* own position            */ var owp   = owu.Position;
            /* own vision range        */ var vr    = owu.VisionRange;
            /* own vision range ^2     */ var vr2   = vr.sqr();
            /* vision + cell radius ^2 */ var vcrp2 = (vr + cr).sqr();
            /* vision - cell radius ^2 */ var vcrm2 = (vr - cr).sqr();
            /* grid vision area        */ var gva   = usg.get_rect_of_circle(owp, vr);

            for (var yi = gva.min.y; yi <= gva.max.y; yi++)
            for (var xi = gva.min.x; xi <= gva.max.x; xi++)
            {
                /* the cell */ var ci = usg.get_index_of(xi, yi);

                if (ci != 0) // is not in the outer cell
                {
                    // cell was already visited and is fully visible
                    if (fvc[ci])
                        continue;
                    
                    /* cell's center                 */ var cc    = ccs[ci];
                    /* delta from unit to cell       */ var cd    = cc - owp;
                    /* distance ^2 from unit to cell */ var cdst2 = cd.sqrMagnitude;

                    //cell is outside the vision area
                    if (cdst2 > vcrp2)
                        continue;

                    // cell is compeletely inside the vision area
                    if (cdst2 <= vcrm2)
                    {
                        /* cell units        */ var cus = sgu[ci];
                        /* cell visibilities */ var cuv = sgv[ci];
                        /* cell units count  */ var cuc = cus.Count;
                        for (int i = 0; i < cuc; i++)
                        {
                            /* other units is visible */ ref var ouv = ref cuv[i];
                            if (ouv) 
                                continue;
                            
                            /* other unit */ var ou = cus[i];
                            ouv = true;
                            ovus.Add(ou);
                        }

                        fvc[ci] = true;
                        continue;
                    }
                }

                // cell is partially inside the vision area
                {
                    /* cell positions    */ var cps = sgp[ci];
                    /* cell visibilities */ var cuv = sgv[ci];
                    /* cell units        */ var cus = sgu[ci];
                    /* cell units count  */ var cuc = cps.Count;

                    for (int i = 0; i < cuc; i++)
                    {
                        ref var ouv = ref cuv[i];
                        if (ouv)
                            continue;

                        /* other unit's position */ var oup = cps[i];
                        if ((oup - owp).sqrMagnitude > vr2) // is outside the range
                            continue;

                        /* other unit */ var ou = cus[i];
                        ouv = true;
                        ovus.Add(ou);
                    }
                }
            }
        }

        ovus.Cleanup();
    }
}