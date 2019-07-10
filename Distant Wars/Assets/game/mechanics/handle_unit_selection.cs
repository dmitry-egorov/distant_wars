using Plugins.Lanski.Behaviours;
using UnityEngine;

public class handle_unit_selection : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying) return;
        
        var /* local player */ lp = LocalPlayer.Instance;

        var /* left mouse button is up */ mu = lp.LeftMouseButtonIsDown;
        var /* finished dragging */       fd = lp.FinishedDragging;

        if (mu || fd)
        {
            var us = lp.UnitsUnderTheCursorBox;
            lp.select_units(us);
        }
    }
}