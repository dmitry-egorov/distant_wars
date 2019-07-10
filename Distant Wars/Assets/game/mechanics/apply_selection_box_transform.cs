public class apply_selection_box_transform : MassiveMechanic
{
    public void _()
    {
        var sb = SelectionBox.Instance;
        var lp = LocalPlayer.Instance;

        if (lp.IsDragging)
        {
            sb.enable(lp.WorldCursorBox);
        }
        else
        {
            sb.disable();
        }
    }
}