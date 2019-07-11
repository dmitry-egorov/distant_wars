internal class prepare_world_mouse_position : MassiveMechanic
{
    public void _()
    {
        var     /* local player */ lp = LocalPlayer.Instance;
        var /* strategic camera */ sc = StrategicCamera.Instance;

        var /* mouse position */ p = lp.ScreenMousePosition;
        var /* multiplier */ m = sc.WorldToScreenMultiplier;
        var     /* offset */ o = sc.WorldToScreenOffset;
        lp.WorldMousePosition = (p - o) / m;
    }
}