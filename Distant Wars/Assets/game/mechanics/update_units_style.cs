public class update_units_style : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;

        foreach (var /* unit */ u in lp.PreviousUnitsInTheCursorBox)
            u.is_highlighted = false;

        foreach (var /* unit */ u in lp.UnitsInTheCursorBox)
            u.is_highlighted = true;

        foreach (var u in lp.PreviouslySelectedUnits)
            u.is_selected = false;

        foreach (var /* unit */ u in lp.SelectedUnits) 
            u.is_selected = true;
    }
}