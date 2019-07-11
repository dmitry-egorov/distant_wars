using System.Collections.Generic;

internal class initialize_local_player : MassiveMechanic
{
    public void _()
    {
        var lp = LocalPlayer.Instance;
        lp.UnitsInTheCursorBox = new List<Unit>();
        lp.PreviousUnitsInTheCursorBox = new List<Unit>();
        lp.SelectedUnits = new List<Unit>();
        lp.PreviouslySelectedUnits = new List<Unit>();
    }
}