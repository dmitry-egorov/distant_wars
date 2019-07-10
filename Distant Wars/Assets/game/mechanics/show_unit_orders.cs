using System.Collections.Generic;

public class show_unit_orders: MassiveMechanic
{
    public void _()
    {
        var lp = LocalPlayer.Instance;

        if (lp.SelectionChanged)
        {
            var psu = PreviouslySelectedUnits;

            foreach (var u in psu) u.hide_order_graphic();
            psu.Clear();

            foreach (var su in lp.SelectedUnits)
            {
                su.show_order_graphic();
                psu.Add(su);
            }
        }

        foreach (var u in Unit.All)
        {
            u.update_order_graphic();
        }
    }
    
    List<Unit> PreviouslySelectedUnits => previously_selected_units ?? (previously_selected_units = new List<Unit>());
    List<Unit> previously_selected_units;
}