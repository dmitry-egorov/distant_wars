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
        
        if (Game.Instance.ManualPhysics)
        {
            var /* mouse position */ mp = lp.WorldMousePosition;
            var    /* boxed units */ bu = lp.UnitsUnderTheCursorBox;
            var   /* closest unit */ cu = default(Unit);
            var /* sqr distance to the closest unit */ sqcd = float.MaxValue;
            foreach (var /* unit */ u in Unit.All)
            {
                var /* sqr distance to the unit */ sqd = (u.Position - mp).sqrMagnitude;
                if (sqd < sqcd)
                {
                    cu = u;
                    sqcd = sqd;
                }
            }

            var     /* units' manager */ um = Units.Instance;
            var       /* units' scale */ us = um.Scale.x;
            var /* selection distance */ sd = um.SelectionDistance;
            if (cu != null && sqcd < (sd * us).sqr())
            {
                bu.Add(cu);
            }
        }
        else
        {
            var /* mouse position */ mp = lp.WorldMousePosition;
            var /* boxed units */    bu = lp.UnitsUnderTheCursorBox;

            var collider = Physics2D.OverlapPoint(mp);
            if (collider.TryGetComponent<Unit>(out var /* unit */ u)) 
            {
                bu.Add(u);
            }
        }
    }
}