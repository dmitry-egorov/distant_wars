using Plugins.Lanski;
using UnityEngine;

public class select_units : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying) return;
        
        var /* local player            */ lp = LocalPlayer.Instance;
        var /* left mouse button is up */ mu = lp.LeftMouseButtonIsDown;
        var /* finished dragging       */ fd = lp.FinishedDragging;

        if (mu || fd)
        {
            var us = lp.UnitsInTheCursorBox;
            
            //swap arrays of selected units
            /* selected units */ var su = lp.PreviouslySelectedUnits;
            su.Clear();
            
            lp.PreviouslySelectedUnits = lp.SelectedUnits;
            lp.SelectedUnits = su;

            foreach (var u in us) 
                su.Add(u);
        }

        lp.PreviouslySelectedUnits.CleanUpExpiredObjects();
        lp.SelectedUnits.CleanUpExpiredObjects();
    }
}