using Plugins.Lanski.Space;
using UnityEngine;

internal class update_units_space_grid : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        /* units' registry */ var ur = UnitsRegistry.Instance;

        var h = ur.SpaceGridHeight;
        var w = ur.SpaceGridWidth;
        var sg = ur.SpaceGrid;
        
        if (sg == null || sg.size.x != w || sg.size.y != h)
        {
            /* half scale */ var hs = map.Scale * 0.5f;
            var rect = new Plugins.Lanski.Space.Rect(-hs.xy(), hs.xy());
            sg = ur.SpaceGrid = new SpaceGrid2<Unit>(rect, new Vector2Int(w, h));
        }

        sg.clear();

        var us = ur.Units;
        foreach (var u in us)
        {
            sg.add(u.Position, u);
        }
    }
}
