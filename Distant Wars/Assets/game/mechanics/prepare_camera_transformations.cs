using Plugins.Lanski.Space;
using UnityEngine;

internal class prepare_camera_transformations: MassiveMechanic
{
    public void _()
    {
        var res = Camera.main.pixelRect;
        var w = (int)res.width;
        var h = (int)res.height;
        
        var sc = StrategicCamera.Instance;
        
        /* camera size */ var ss = sc.Size;
        /* screen size */
        var sr = new Vector2(w, h);
        var scs = 0.5f * sr;
        var m = scs.y / ss;
        var o = scs - sc.Position * m;

        sc.ScreenResolution = sr;
        sc.WorldToScreenTransform = new UniformSpaceTransform2(m, o);
        var s2w = sc.ScreenToWorldTransform = new UniformSpaceTransform2(1 / m, -o / m);

        var wsmin = s2w.apply_to_point(new Vector2(0, 0));
        var wsmax = s2w.apply_to_point(sr);
        sc.WorldScreen = new Plugins.Lanski.Space.Rect(wsmin, wsmax);
    }
}