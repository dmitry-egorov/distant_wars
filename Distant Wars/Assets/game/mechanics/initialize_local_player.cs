using Plugins.Lanski;

internal class init_local_player : IMassiveMechanic
{
    public void _()
    {
        var lp = LocalPlayer.Instance;
        lp.UnitsInTheCursorBox = new RefLeakyList<Unit>();
        lp.PreviousUnitsInTheCursorBox = new RefLeakyList<Unit>();
        lp.SelectedUnits = new RefLeakyList<Unit>();
        lp.PreviouslySelectedUnits = new RefLeakyList<Unit>();
    }
}