using Plugins.Lanski;
using UnityEngine;

public class find_units_under_the_cursor : MassiveMechanic
{
    public void _()
    {
        /* units' repository */ var ur = UnitsRegistry.Instance;
        /* local player      */ var lp = LocalPlayer.Instance;
        /* units in the box  */ var bu = lp.PreviousUnitsInTheCursorBox;
        bu.Clear();
        
        lp.PreviousUnitsInTheCursorBox = lp.UnitsInTheCursorBox;
        lp.UnitsInTheCursorBox = bu;

        /* cursor is a box */ var ib = lp.IsDragging || lp.FinishedDragging;

        var vus = ur.VisionUnits;
        var ous = ur.VisibleOtherUnits;

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
            /* multiplier       */ var w2st = sc.WorldToScreenTransform;
            /* mouse position   */ var mp   = lp.ScreenMousePosition;

            var /* closest unit */ cu = default(Unit);
            var /* sqr distance to the closest unit */ sqcd = float.MaxValue;

            //PERF: use space grid
            for (int i = 0; i < tuc; i++)
            {
                /* unit */ var u = i < vuc ? vus[i] : ous[i - vuc];
                /* world space unit position  */ var  wp = u.Position;
                /* screen space unit position */ var  sp = w2st.apply_to_point(wp);
                /* sqr distance to the unit   */ var sqd = (sp - mp).sqrMagnitude;
                if (sqd < sqcd)
                {
                    cu = u;
                    sqcd = sqd;
                }
            }

            var /* units' screen size */ uss = ur.SpriteSize;
            var /* selection distance */ sd  = ur.ScreenSelectionDistance;
            var /* adjusted selection distance */ asd = sd * uss * 0.5f;
            if (cu != null && sqcd < asd.sqr())
                bu.Add(cu);
        }
    }
}