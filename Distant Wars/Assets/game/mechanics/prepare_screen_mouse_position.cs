using UnityEngine;

internal class prepare_screen_mouse_position : IMassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;
        var /* screen mouse position */ sp = Input.mousePosition.xy();
        lp.ScreenMousePosition = sp;
    }
}