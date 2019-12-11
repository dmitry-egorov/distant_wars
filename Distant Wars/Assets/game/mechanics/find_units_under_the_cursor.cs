using Plugins.Lanski;
using UnityEngine;

public class find_units_under_the_cursor : MassiveMechanic
{
    public void _()
    {
        /* units' repository   */ var ur   = UnitsRegistry.Instance;
        /* space grid          */ var sg   = ur.SpaceGrid;
        /* cells unit postions */ var csps = sg.cells_positions;
        /* cells unit postions */ var csvs = sg.cells_visibilities;
        /* cells units         */ var csus = sg.cells_units;
        /* cells centers       */ var cscs = sg.cells_centers;

        /* local player      */ var lp = LocalPlayer.Instance;
        /* units in the box  */ var bu = lp.PreviousUnitsInTheCursorBox;
        bu.Clear();
        
        lp.PreviousUnitsInTheCursorBox = lp.UnitsInTheCursorBox;
        lp.UnitsInTheCursorBox = bu;

        /* cursor is a box */ var ib = lp.IsDragging || lp.FinishedDragging;

        var vus = ur.OwnTeamUnits;
        var ous = ur.OtherTeamsVisibleUnits;

        var vuc = vus.Count;
        var ouc = ous.Count;
        var tuc = vuc + ouc;
        
        if (ib)
        {
            var /* cursor box */ cb = lp.WorldCursorBox;

            var min = cb.min;
            var max = cb.max;

            var minx = Mathf.Min(min.x, max.x);
            var maxx = Mathf.Max(min.x, max.x);
            var miny = Mathf.Min(min.y, max.y);
            var maxy = Mathf.Max(min.y, max.y);

            //PERF: use space grid
            for (int i = 0; i < tuc; i++)
            {
                var /* unit */ u = i < vuc ? vus[i] : ous[i - vuc];
                var p = u.Position;
                var px = p.x;
                var py = p.y;
                var /* is within the box */ wb =
                    px >= minx
                    && px <= maxx
                    && py >= miny
                    && py <= maxy
                ;
                if (wb)
                    bu.Add(u);
            }
        }
        else
        {
            /* strategic camera */ var sc   = StrategicCamera.Instance;
            /* multiplier       */ var s2wt = sc.ScreenToWorldTransform;
            /* mouse position   */ var mpos = lp.WorldMousePosition;

            /* units' screen size                */ var uss  = ur.SpriteSize;
            /* selection distance                */ var sd   = ur.ScreenSelectionDistance;
            /* adjusted selection distance       */ var asd  = sd * uss * 0.5f;
            /* world space selection distance    */ var wsd  = s2wt.apply_to_scalar(asd);
            /* world space selection distance ^2 */ var wsd2 = wsd.sqr();

            /* closest unit */ var cu = default(Unit);
            /* min distance to the closest unit ^2 */ var mind2 = float.MaxValue;
            /* grid vision area */ var gva = sg.get_rect_of_circle(mpos, wsd);
            for (var yi = gva.min.y; yi <= gva.max.y; yi++)
            for (var xi = gva.min.x; xi <= gva.max.x; xi++)
            {
                /* cell index        */ var ci  = sg.get_index_of(xi, yi);
                /* cell positions    */ var cps = csps[ci];
                /* cell visibilities */ var cvs = csvs[ci];
                /* cell units        */ var cus = csus[ci];
                /* cell units count  */ var cuc = cps.Count;

                for (var i = 0; i < cuc; i++)
                {
                    /* cell visibilities */ var cuv = cvs[i];
                    if (!cuv)
                        continue;
                    
                    /* world space unit position */ ref var pos = ref cps[i];
                    /* sqr distance to the unit  */ var dist2 = (pos - mpos).sqrMagnitude;
                    if (dist2 < mind2 && dist2 < wsd2)
                    {
                        /* potential unit */ var pu = cus[i];
                        cu = pu;
                        mind2 = dist2;
                    }
                }
            }
            
            if (cu != null)
            {
                bu.Add(cu);
            }
        }
    }
}