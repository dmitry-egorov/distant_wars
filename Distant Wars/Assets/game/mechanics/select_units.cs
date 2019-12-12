using Plugins.Lanski;
using UnityEngine;

public class select_units : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying) return;
        
        var /* local player              */ lp = LocalPlayer.Instance;
        var /* left mouse button is down */ lmb_down = lp.LeftMouseButtonIsDown;
        var /* finished dragging         */ fd = lp.FinishedDragging;

        if (lmb_down || fd)
        {
            var cursor_units = lp.UnitsInTheCursorBox;
            
            //swap arrays of selected units
            var selected_units = lp.PreviouslySelectedUnits;
            selected_units.Clear();
            
            lp.PreviouslySelectedUnits = lp.SelectedUnits;
            lp.SelectedUnits = selected_units;

            foreach (var u in cursor_units) 
                selected_units.Add(u);
            
            selected_units.Cleanup();
        }

        lp.PreviouslySelectedUnits.CleanUpExpiredObjects();
        lp.SelectedUnits.CleanUpExpiredObjects();
    }
}