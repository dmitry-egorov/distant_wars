using Plugins.Lanski;
using Plugins.Lanski.Space;
using UnityEngine;

public class find_units_under_the_cursor : MassiveMechanic
{
    public void _()
    {
        var time_ratio = Game.Instance.PresentationToSimulationFrameTimeRatio;

        /* units' repository   */ var ur = UnitsRegistry.Instance;

        /* space grid */ var sg = ur.SpaceGrid;
        /* cells unit postions      */ var poss  = sg.unit_positions;
        /* cells unit prev postions */ var pposs = sg.unit_prev_positions;
        /* cells unit visibilities  */ var viss  = sg.unit_detections_by_team;
        /* cells units              */ var units = sg.unit_refs;

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

            /* grid vision area iterator */ var gva = sg.get_iterator_of(proper_rect);
            while (gva.next(out var cell_i))
            {
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
            /* grid vision area iterator */ var it = sg.get_iterator_of_circle(mouse_pos, wsd);
            while (it.next(out var cell_i))
            {
                /* cell unit positions      */ var cuposs  = poss [cell_i];
                /* cell unit prev positions */ var cupposs = pposs[cell_i];
                /* cell unit visibilities   */ var cuviss  = viss [cell_i];
                /* cell units               */ var cunits  = units[cell_i];
                /* cell units count         */ var cucount = cuposs.Count;

                for (var i = 0; i < cucount; i++)
                {
                    /* cell visibilities */ var cvis = cuviss[i];
                    if ((cvis & pteam_mask) == 0)
                        continue;
                    
                    /* unit's position      */ var upos  = cuposs [i];
                    /* unit's prev position */ var uppos = cupposs[i];
                    /* unit's interpolated position */ var uipos = Vector2.Lerp(uppos, upos, time_ratio);
                    
                    /* sqr distance to the unit  */ var dist2 = (uipos - mouse_pos).sqrMagnitude;
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