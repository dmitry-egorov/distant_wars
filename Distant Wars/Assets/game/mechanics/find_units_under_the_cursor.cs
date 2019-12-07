using Plugins.Lanski;
using UnityEngine;

public class find_units_under_the_cursor : MassiveMechanic
{
    public void _()
    {
        var /* units' repository */ ur = UnitsRegistry.Instance;
        var      /* local player */ lp = LocalPlayer.Instance;

        var /* units in the box */ bu = lp.PreviousUnitsInTheCursorBox;
        bu.Clear();
        
        lp.PreviousUnitsInTheCursorBox = lp.UnitsInTheCursorBox;
        lp.UnitsInTheCursorBox = bu;

        var /* cursor is a box */ ib = lp.IsDragging || lp.FinishedDragging;

        var us = ur.OwnUnits;
        
        if (ib)
        {
            var /* cursor box */ cb = lp.WorldCursorBox;

            var min = cb.min;
            var max = cb.max;

            var minx = Mathf.Min(min.x, max.x);
            var maxx = Mathf.Max(min.x, max.x);
            var miny = Mathf.Min(min.y, max.y);
            var maxy = Mathf.Max(min.y, max.y);

            foreach (var /* unit */ u in us)
            {
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
            var  /* strategic camera */   sc = StrategicCamera.Instance;
            var        /* multiplier */  w2s = sc.WorldToScreenMultiplier;
            var            /* offset */ w2so = sc.WorldToScreenOffset;
            var    /* mouse position */   mp = lp.ScreenMousePosition;

            var      /* closest unit */   cu = default(Unit);
            var /* sqr distance to the closest unit */ sqcd = float.MaxValue;
            foreach (var /* unit */ u in us)
            {
                var  /* world space unit position */  wp = u.Position;
                var /* screen space unit position */  sp = wp * w2s + w2so;
                var   /* sqr distance to the unit */ sqd = (sp - mp).sqrMagnitude;
                if (sqd < sqcd)
                {
                    cu = u;
                    sqcd = sqd;
                }
            }

            var /* units' screen size */ uss = ur.WorldScale * w2s;
            var /* selection distance */  sd = ur.ScreenSelectionDistance;
            var /* adjusted selection distance */ asd = sd * uss / 2f;
            if (cu != null && sqcd < asd.sqr())
                bu.Add(cu);
        }
    }
}