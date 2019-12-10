using System;
using Plugins.Lanski;
using UnityEngine;
using Rect = Plugins.Lanski.Space.Rect;


// find visible units other than the player's
internal class update_visible_other_units : MassiveMechanic
{
    public void _()
    {
        // assuming own units are marked as visible, and are never added to the VisibleOtherUnits list

        var ur = UnitsRegistry.Instance;
        var ovu = ur.VisibleOtherUnits;

        foreach (var vu in ovu)
        {
            vu.IsVisible = false;
        }

        ovu.Clear();
        /* unit's space grid   */ var usg  = ur.SpaceGrid;
        /* cell's radius       */ var cr   = usg.cell_radius;
        /* cells unit postions */ var csps = usg.cells_positions;
        /* cells units         */ var csus = usg.cells_elements;
        /* cells centers       */ var cscs = usg.cells_centers;

        /* fully visible cells */ var fvc = fully_visible_cells;
        if (fvc == null || fvc.Length != csps.Length)
        {
            fully_visible_cells = fvc = new bool[csps.Length];
        }
        else
        {
            Array.Clear(fvc, 0, fvc.Length);
        }

        foreach (/* own unit */ var owu in ur.VisionUnits)
        {
            /* own position            */ var owp   = owu.Position;
            /* own vision range        */ var vr    = owu.VisionRange;
            /* own vision range ^2     */ var vr2   = vr.sqr();
            /* vision + cell radius ^2 */ var vcrp2 = (vr + cr).sqr();
            /* vision - cell radius ^2 */ var vcrm2 = (vr - cr).sqr();
            /* vision area             */ var va    = new Rect(owp + new Vector2(-vr, -vr), owp + new Vector2(vr, vr));
            /* grid vision area        */ var gva   = usg.get_rect_of(va);

            for (var yi = gva.min.y; yi <= gva.max.y; yi++)
            for (var xi = gva.min.x; xi <= gva.max.x; xi++)
            {
                /* the cell */ var ci = usg.get_index_of(xi, yi);

                if (ci != 0) // is not in the outer cell
                {
                    // cell was already visited and is fully visible
                    if (fvc[ci])
                        continue;
                    
                    /* cell's center                 */ var cc    = cscs[ci];
                    /* delta from unit to cell       */ var cd    = cc - owp;
                    /* distance ^2 from unit to cell */ var cdst2 = cd.sqrMagnitude;

                    //cell is outside the vision area
                    if (cdst2 > vcrp2)
                        continue;

                    // cell is compeletely inside the vision area
                    if (cdst2 <= vcrm2)
                    {
                        /* cell units       */ var cus = csus[ci];
                        /* cell units count */ var cuc = cus.Count;
                        for (int i = 0; i < cuc; i++)
                        {
                            /* other unit */ var ou = cus[i];
                            if (ou.IsVisible)
                                continue;

                            ou.IsVisible = true;
                            ovu.Add(ou);
                        }

                        fvc[ci] = true;
                        continue;
                    }
                }

                // cell is partially inside the vision area
                {
                    /* cell positions   */ var cps  = csps[ci];
                    /* cell units       */ var cus  = csus[ci];
                    /* cell units count */ var cuc  = cps.Count;

                    for (int i = 0; i < cuc; i++)
                    {
                        /* other unit */ var ou = cus[i];
                        if (ou.IsVisible) // is already visible
                            continue;

                        /* other unit's position */ var oup = cps[i];
                        if ((oup - owp).sqrMagnitude > vr2) // is outside the range
                            continue;

                        ou.IsVisible = true;
                        ovu.Add(ou);
                    }
                }
            }
        }

        ovu.Cleanup();
    }

    bool[] fully_visible_cells;
}