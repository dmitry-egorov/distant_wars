using UnityEngine;

internal class hide_vision_on_key : IMassiveMechanic
{
    public void _()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            var r = UnitsRegistry.Instance.VisionCirclesRenderer;
            r.enabled = !r.enabled;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            var r = UnitsRegistry.Instance.VisionQuadsRenderer;
            r.enabled = !r.enabled;
        }
    }
}