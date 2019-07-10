using UnityEngine;

internal class prepare_mouse_clicking : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;
        lp.LeftMouseButtonIsDown = Input.GetMouseButtonDown(0);
        lp.LeftMouseButtonIsHeld = Input.GetMouseButton(0);
        lp.LeftMouseButtonIsUp = Input.GetMouseButtonUp(0);
        lp.RightMouseButtonIsDown = Input.GetMouseButtonDown(1);
        lp.RightMouseButtonIsHeld = Input.GetMouseButton(1);
        lp.RightMouseButtonIsUp = Input.GetMouseButtonUp(1);
    }
}