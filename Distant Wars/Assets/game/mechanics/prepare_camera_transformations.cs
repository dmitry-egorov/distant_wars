using Plugins.Lanski.Space;
using UnityEngine;

internal class prepare_camera_transformations: IMassiveMechanic
{
    public void _()
    {
        var res = Camera.main.pixelRect;
        var w = (int)res.width;
        var h = (int)res.height;
        
        var sc = StrategicCamera.Instance;
        
        /* camera size      */ var csize = sc.Size;
        /* screen size      */ var ssize = new Vector2(w, h);
        /* half screen size */ var hssize = 0.5f * ssize;
        /* multiplier       */ var m = hssize.y / csize;
        /* offset           */ var o = hssize - sc.Position * m;

        sc.ScreenResolution = new Vector2Int(w, h);
        sc.SpriteSizeMultiplier = h / 1080;

        sc.WorldToScreenTransform = new UniformSpaceTransform2(m, o);
        var s2w = sc.ScreenToWorldTransform = new UniformSpaceTransform2(1 / m, -o / m);

        var wsmin = s2w.apply_to_point(new Vector2(0, 0));
        var wsmax = s2w.apply_to_point(ssize);
        sc.WorldScreen = new FRect(wsmin, wsmax);
    }
}