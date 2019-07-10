using UnityEngine;

internal class prepare_mouse_position : MassiveMechanic
{
    public void _()
    {
        var /* local player */ lp = LocalPlayer.Instance;
        var /* screen mouse position */ sp = Input.mousePosition.xy();
        lp.ScreenMousePosition = sp;
        StrategicCamera temp_qualifier = StrategicCamera.Instance;
        lp.WorldMousePosition = temp_qualifier.Camera.ScreenToWorldPoint(sp.xy0()).xy();
    }
}