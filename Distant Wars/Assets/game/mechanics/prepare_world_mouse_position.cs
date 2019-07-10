internal class prepare_world_mouse_position : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;
        var /* startegic camera */ sc = StrategicCamera.Instance;

        var /* mouse position */ p = lp.ScreenMousePosition;
        var /* multiplier */ m = sc.WorldToScreenSpaceMultiplier;
        var     /* offset */ o = sc.WorldToScreenSpaceOffset;
        lp.WorldMousePosition = (p - o) / m;
    }
}