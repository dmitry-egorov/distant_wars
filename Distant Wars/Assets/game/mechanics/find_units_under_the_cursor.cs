using Plugins.Lanski;
using Plugins.Lanski.Space;
using UnityEngine;

public class find_units_under_the_cursor : MassiveMechanic
{
    public void _()
    {
        /* units' repository   */ var ur = UnitsRegistry.Instance;
        /* space grid          */ var sg = ur.SpaceGrid;
        /* cells unit postions */ var poss  = sg.unit_positions;
        /* cells unit postions */ var viss  = sg.unit_visibilities;
        /* cells units         */ var units = sg.unit_refs;
        /* cells centers       */ var ccenters = sg.cell_centers;

        /* local player      */ var lp = LocalPlayer.Instance;
        /* player team mask  */ var pteam_mask = lp.Faction.Team.Mask;  
        /* units in the box  */ var box_units = lp.PreviousUnitsInTheCursorBox;
        box_units.Clear();
        
        lp.PreviousUnitsInTheCursorBox = lp.UnitsInTheCursorBox;
        lp.UnitsInTheCursorBox = box_units;

        /* cursor is a box */ var ib = lp.IsDragging || lp.FinishedDragging;
        
        if (ib)
        {
            /* cursor box */ var cb = lp.WorldCursorBox;
            var min = cb.min;
            var max = cb.max;

            var bminx = Mathf.Min(min.x, max.x);
            var bmaxx = Mathf.Max(min.x, max.x);
            var bminy = Mathf.Min(min.y, max.y);
            var bmaxy = Mathf.Max(min.y, max.y);

            var proper_rect = new FRect(bminx, bminy, bmaxx, bmaxy);

            /* grid vision area */ var gva = sg.get_coord_rect_of(proper_rect);
            var minx = gva.min.x;
            var miny = gva.min.y;
            var maxx = gva.max.x;
            var maxy = gva.max.y;

            for (var yi = miny; yi <= maxy; yi++)
            for (var xi = minx; xi <= maxx; xi++)
            {
                /* cell index             */ var cell_i = sg.get_index_of(xi, yi);
                /* cell unit positions    */ var cuposs = poss[cell_i];
                /* cell unit visibilities */ var cuviss = viss[cell_i];
                /* cell units        */ var cunits  = units[cell_i];
                /* cell units count  */ var cucount = cuposs.Count;

                for (var unit_i = 0; unit_i < cucount; unit_i++)
                {
                    /* cell visibilities */ var cvis = cuviss[unit_i];
                    if ((cvis & pteam_mask) == 0)
                        continue;

                    var p = cuposs[unit_i];
                    var px = p.x;
                    var py = p.y;

                    var /* is within the box */ wb =
                        px >= bminx
                        && px <= bmaxx
                        && py >= bminy
                        && py <= bmaxy
                    ;
                    if (wb)
                    {
                        var u = cunits[unit_i];
                        box_units.Add(u);
                    }
                }
            }
        }
        else
        {
            /* strategic camera */ var sc = StrategicCamera.Instance;
            /* screen to world transform  */ var s2w = sc.ScreenToWorldTransform;
            /* world space mouse position */ var mouse_pos = lp.WorldMousePosition;

            /* units' screen size */ var ussize = ur.SpriteSize;
            /* screen selection distance         */ var sd  = ur.ScreenSelectionDistance;
            /* adjusted selection distance       */ var asd = sd * ussize * 0.5f;
            /* world space selection distance    */ var wsd = s2w.apply_to_scalar(asd);
            /* world space selection distance ^2 */ var sel_dist_2 = wsd.sqr();

            /* closest unit */ var cunit = default(Unit);
            /* min distance to the closest unit ^2 */ var min_dist_2 = float.MaxValue;
            /* grid vision area */ var g_vis_area = sg.get_coord_rect_of_circle(mouse_pos, wsd);
            var minx = g_vis_area.min.x;
            var miny = g_vis_area.min.y;
            var maxx = g_vis_area.max.x;
            var maxy = g_vis_area.max.y;

            for (var yi = miny; yi <= maxy; yi++)
            for (var xi = minx; xi <= maxx; xi++)
            {
                /* cell index             */ var cell_i  = sg.get_index_of(xi, yi);
                /* cell unit positions    */ var cuposs   = poss[cell_i];
                /* cell unit visibilities */ var cuviss  = viss[cell_i];
                /* cell units        */ var cunits  = units[cell_i];
                /* cell units count  */ var cucount = cuposs.Count;

                for (var i = 0; i < cucount; i++)
                {
                    /* cell visibilities */ var cvis = cuviss[i];
                    if ((cvis & pteam_mask) == 0)
                        continue;
                    
                    /* world space unit position */ ref var pos = ref cuposs[i];
                    /* sqr distance to the unit  */ var dist2 = (pos - mouse_pos).sqrMagnitude;
                    if (dist2 < min_dist_2 && dist2 < sel_dist_2)
                    {
                        /* potential unit */ var pu = cunits[i];
                        cunit = pu;
                        min_dist_2 = dist2;
                    }
                }
            }
            
            if (cunit != null)
                box_units.Add(cunit);
        }

        box_units.Cleanup();
    }
}