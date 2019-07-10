using System.Collections.Generic;
using UnityEngine;

public class highlight_units : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying) return;
        
        var /* local player */ lp = LocalPlayer.Instance;
        
        var id = lp.IsDragging;
        var bu = lp.UnitsUnderTheCursorBox;

        var /* previously highlighted units */ phu = PreviouslyHighlightedUnits;

        // clear previously highlighted units
        {
            foreach (var unit in phu) unit.apply_default_material();
            phu.Clear();
        }

        // highlight selected units
        foreach (var unit in lp.SelectedUnits)
        {
            unit.apply_highlighted_material();
            phu.Add(unit);
        }

        // highlight units in the selection box
        if (id)
        foreach (var unit in bu)
        {
            unit.apply_highlighted_material();
            phu.Add(unit);
        }
    }
    
    List<Unit> PreviouslyHighlightedUnits => previously_highlighted_units ?? (previously_highlighted_units = new List<Unit>());
    List<Unit> previously_highlighted_units;
}