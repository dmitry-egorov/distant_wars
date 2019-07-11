using Plugins.Lanski;

internal class prepare_camera_transformations: MassiveMechanic
{
    public void _()
    {
        var sc = StrategicCamera.Instance;
        
        var /* camera size */ ss = sc.Size;
        var /* screen size */ scs = 0.5f * ScreenEx.Size.v2f();
        var m = scs.y / ss;
        var o = scs - sc.Position * m;

        sc.WorldToScreenMultiplier = m;
        sc.WorldToScreenOffset = o;
    }
}