using UnityEngine;

internal class prepare_camera_transformations: MassiveMechanic
{
    public void _()
    {
        var sc = StrategicCamera.Instance;
        var c = sc.Camera;
        
        var /* camera size */ ss = sc.Size;
        var /* screen size */ scs = new Vector2(Screen.width, Screen.height);
        var vss = new Vector2(2 * ss * c.aspect, 2 * ss);
        var m = scs / vss;
        var o = new Vector2(0.5f, 0.5f) * scs - sc.Position * m;

        sc.ScreenSpaceMultiplier = m;
        sc.ScreenSpaceOffset = o;
    }
}