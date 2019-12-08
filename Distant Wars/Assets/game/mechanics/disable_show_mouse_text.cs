internal class debug_show_mouse_text : MassiveMechanic
{
    public void _() 
    {
        var mwp = LocalPlayer.Instance.WorldMousePosition;
        DebugText.set_text("mouse pos", mwp.ToString());
        DebugText.set_text("mouse coord", Map.Instance.coord_of(mwp).ToString());
        DebugText.set_text("height at mouse", Map.Instance.z(mwp).ToString());
    }
}