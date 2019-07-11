using System.Collections.Generic;
using UnityEngine;

public class set_units_style : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;

        foreach (var u in lp.PreviouslySelectedUnits)
        {
            u.IsSelected = false;
            u.set_default_style();
        }

        foreach (var /* unit */ u in lp.PreviousUnitsInTheCursorBox)
        {
            u.IsHighlighted = false;
            u.set_default_style();
        }

        // highlight units in the cursor box
        var /* units under the cursor box */ bu = lp.UnitsInTheCursorBox;
        foreach (var /* unit */ u in bu)
        {
            u.IsHighlighted = true;
            u.set_highlighted_style();
        }

        // highlight selected units
        var /* selected units */ su = lp.SelectedUnits;
        foreach (var /* unit */ u in su)
        {
            u.IsSelected = true;
            u.set_selected_style();
        }
    }
}