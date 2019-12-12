using UnityEngine;

internal class hide_vision_quads_on_key : MassiveMechanic
{
    public void _()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            var vr = UnitsRegistry.Instance.VisionRenderer;
            vr.enabled = !vr.enabled;
        }
    }
}