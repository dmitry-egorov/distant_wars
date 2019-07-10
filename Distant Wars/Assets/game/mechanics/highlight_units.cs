using System.Collections.Generic;
using UnityEngine;

public class highlight_units : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying) return;
        
        var /* local player */ lp = LocalPlayer.Instance;
        

        var /* previously highlighted units */ phu = PreviouslyHighlightedUnits;

        // clear previously highlighted units
        {
            foreach (var unit in phu) unit.apply_default_material();
            phu.Clear();
        }

        // highlight units in the cursor box
        var /* units under the cursor box */ bu = lp.UnitsUnderTheCursorBox;
        foreach (var /* unit */ u in bu)
        {
            u.apply_highlighted_material();
            phu.Add(u);
        }

        // highlight selected units
        var /* selected units */ su = lp.SelectedUnits;
        foreach (var /* unit */ u in su)
        {
            u.apply_selected_material();
            phu.Add(u);
        }
    }
    
    List<Unit> PreviouslyHighlightedUnits => previously_highlighted_units ?? (previously_highlighted_units = new List<Unit>());
    List<Unit> previously_highlighted_units;
}