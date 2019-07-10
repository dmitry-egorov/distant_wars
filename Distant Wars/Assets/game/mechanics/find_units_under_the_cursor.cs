using Plugins.Lanski;
using Plugins.Lanski.Behaviours;
using UnityEngine;

public class find_units_under_the_cursor : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying) return;

        var /* local player */ lp = LocalPlayer.Instance;
        
        var /* finished dragging */ fd = lp.FinishedDragging;
        var /* is dragging */       id = lp.IsDragging;
            
        if (fd || id)
            return;

        var /* units' repository */ ur = UnitsRegistry.Instance;
        var  /* strategic camera */ sc = StrategicCamera.Instance;
        var      /* multiplier */  w2s = sc.WorldToScreenSpaceMultiplier;
        var      /* multiplier */ w2so = sc.WorldToScreenSpaceOffset;
        var    /* mouse position */ mp = lp.ScreenMousePosition;
        var       /* boxed units */ bu = lp.UnitsUnderTheCursorBox;
        var      /* closest unit */ cu = default(Unit);
        var /* sqr distance to the closest unit */ sqcd = float.MaxValue;
        foreach (var /* unit */ u in ur.All)
        {
            var     /* world space unit position */  wp = u.Position;
            var    /* screen space unit position */  sp = wp * w2s + w2so;
            var      /* sqr distance to the unit */ sqd = (sp - mp).sqrMagnitude;
            if (sqd < sqcd)
            {
                cu = u;
                sqcd = sqd;
            }
        }

        var /* units' screen size */ us = ur.WorldScale * w2s;
        var /* selection distance */ sd = ur.ScreenSelectionDistance;
        var /* adjusted selection distance */ asd = sd * us / 2f;
        if (cu != null && sqcd < asd.sqr()) 
            bu.Add(cu);
    }
}