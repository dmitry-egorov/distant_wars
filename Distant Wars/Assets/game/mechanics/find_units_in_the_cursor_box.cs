using UnityEngine;

public class find_units_in_the_cursor_box : MassiveMechanic
{
    public void _()
    {
        if (!Application.isPlaying) return;

        var /* local player */ lp = LocalPlayer.Instance;
            
        var /* finished dragging */ fd = lp.FinishedDragging;
        var /* is dragging */       id = lp.IsDragging;
            
        if (!fd && !id)
            return;

        var /* selection box */ b = lp.WorldCursorBox;
        var /* boxed units */  bu = lp.UnitsUnderTheCursorBox;

        var min = b.min;
        var max = b.max;

        var minx = Mathf.Min(min.x, max.x);
        var maxx = Mathf.Max(min.x, max.x);
        var miny = Mathf.Min(min.y, max.y);
        var maxy = Mathf.Max(min.y, max.y);

        var /* units' repository */ ur = UnitsRegistry.Instance;
        foreach (var u in ur.All)
        {
            var p = u.Position;
            var px = p.x;
            var py = p.y;
            var /* is within the box */ wb =
                px >= minx
                && px <= maxx
                && py >= miny
                && py <= maxy
            ;
            if (wb)
                bu.Add(u);
        }
    }
}