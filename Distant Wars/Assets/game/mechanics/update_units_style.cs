public class update_units_style : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;

        foreach (var /* unit */ u in lp.PreviousUnitsInTheCursorBox)
            u.IsHighlighted = false;

        foreach (var /* unit */ u in lp.UnitsInTheCursorBox)
            u.IsHighlighted = true;

        foreach (var u in lp.PreviouslySelectedUnits)
            u.IsSelected = false;

        foreach (var /* unit */ u in lp.SelectedUnits) 
            u.IsSelected = true;
    }
}