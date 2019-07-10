using System.Collections.Generic;
using UnityEngine;

public class highlight_units : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;

        var /* previously highlighted units */ phu = PreviouslyHighlightedUnits;

        // clear previously highlighted units
        {
            foreach (var unit in phu) 
                unit.set_default_style();
            
            phu.Clear();
        }

        // highlight units in the cursor box
        var /* units under the cursor box */ bu = lp.UnitsUnderTheCursorBox;
        foreach (var /* unit */ u in bu)
        {
            u.make_highlighted_style();
            phu.Add(u);
        }
        
        if (!Application.isPlaying) return;

        // highlight selected units
        var /* selected units */ su = lp.SelectedUnits;
        foreach (var /* unit */ u in su)
        {
            u.set_selected_style();
            phu.Add(u);
        }
    }
    
    List<Unit> PreviouslyHighlightedUnits => previously_highlighted_units ?? (previously_highlighted_units = new List<Unit>());
    List<Unit> previously_highlighted_units;
}