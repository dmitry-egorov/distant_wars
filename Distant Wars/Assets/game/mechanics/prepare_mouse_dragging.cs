using Plugins.Lanski;
using UnityEngine;

public class prepare_mouse_dragging : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;
        
        lp.FinishedDragging = false;

        var /* mouse is down */ id = lp.LeftMouseButtonIsDown;
        var /* mouse is held */ ih = lp.LeftMouseButtonIsHeld;
        
        if (id)
        {
            lp.ScreenDragStartPosition = lp.ScreenMousePosition;
            lp.WorldDragStartPosition = lp.WorldMousePosition;
        }
        else if (ih && !lp.IsDragging) //is holding the mouse button in place
        {
            var    /* selection box */ sb = lp.ScreenCursorBox;
            var /* min box distance */ md = lp.CursorBoxMinDistance;
            if ((sb.max - sb.min).sqrMagnitude > md.sqr())
            {
                lp.IsDragging = true;
            }
        }

        if (lp.IsDragging && lp.LeftMouseButtonIsUp)
        {
            lp.IsDragging = false;
            lp.FinishedDragging = true;
        }

        lp.ScreenCursorBox = new Rect { min = lp.ScreenDragStartPosition, max = lp.ScreenMousePosition };
        lp.WorldCursorBox = new Rect { min = lp.WorldDragStartPosition, max = lp.WorldMousePosition};
    }
}