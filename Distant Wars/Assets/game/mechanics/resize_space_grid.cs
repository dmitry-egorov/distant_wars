using UnityEngine;
using Rect = Plugins.Lanski.Space.Rect;

internal class resize_space_grid : MassiveMechanic
{
    public void _()
    {
        /* units' registry */ var ur  = UnitsRegistry.Instance;
        /* map             */ var map = Map.Instance;

        var h  = ur.SpaceGridHeight;
        var w  = ur.SpaceGridWidth;
        var sg = ur.SpaceGrid;

        if (sg == null || sg.size.x != w || sg.size.y != h)
        {
            /* half scale */ var hs   = map.Scale * 0.5f;
            /* map area   */ var rect = new Rect(-hs.xy(), hs.xy());
            sg = ur.SpaceGrid = new SpaceGrid2(rect, new Vector2Int(w, h));
        }
    }
}