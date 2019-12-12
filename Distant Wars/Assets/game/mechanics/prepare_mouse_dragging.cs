using Plugins.Lanski;
using Plugins.Lanski.Space;

public class prepare_mouse_dragging : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;
        
        lp.FinishedDragging = false;

        var /* mouse is down */ is_down = lp.LeftMouseButtonIsDown;
        var /* mouse is held */ is_held = lp.LeftMouseButtonIsHeld;
        
        if (is_down)
        {
            lp.ScreenDragStartPosition = lp.ScreenMousePosition;
            lp.WorldDragStartPosition = lp.WorldMousePosition;
        }
        else if (is_held && !lp.IsDragging) //is holding the mouse button in place
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

        lp.ScreenCursorBox = new FRect { min = lp.ScreenDragStartPosition, max = lp.ScreenMousePosition };
        lp.WorldCursorBox = new FRect { min = lp.WorldDragStartPosition, max = lp.WorldMousePosition};
    }
}