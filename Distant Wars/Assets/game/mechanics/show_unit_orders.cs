using System.Collections.Generic;

public class show_unit_orders: MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;

        if (lp.SelectionChanged)
        {
            var /* previously selected units */ psu = PreviouslySelectedUnits;

            foreach (var u in psu) u.hide_order_graphic();
            psu.Clear();

            foreach (var /* selected unit */ su in lp.SelectedUnits) 
                psu.Add(su);
        }

        foreach (var u in lp.SelectedUnits)
        {
            u.show_order_graphic();
        }
    }
    
    List<Unit> PreviouslySelectedUnits => previously_selected_units ?? (previously_selected_units = new List<Unit>());
    List<Unit> previously_selected_units;
}