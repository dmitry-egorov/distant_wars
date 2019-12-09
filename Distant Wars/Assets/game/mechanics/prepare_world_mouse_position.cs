internal class prepare_world_mouse_position : MassiveMechanic
{
    public void _()
    {
        var     /* local player */ lp = LocalPlayer.Instance;
        var /* strategic camera */ sc = StrategicCamera.Instance;

        var /* mouse position */ p = lp.ScreenMousePosition;
        var /* multiplier     */ t = sc.ScreenToWorldTransform;
        lp.WorldMousePosition = t.apply_to_point(p);
    }
}