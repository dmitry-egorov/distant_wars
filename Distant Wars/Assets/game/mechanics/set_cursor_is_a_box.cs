internal class set_cursor_is_a_box : IMassiveMechanic
{
    public void _()
    {
        var lp = LocalPlayer.Instance;
        lp.CursorIsABox = lp.IsDragging || lp.FinishedDragging;
    }
}