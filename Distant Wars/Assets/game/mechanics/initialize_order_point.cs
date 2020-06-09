using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class init_order_point : IMassiveMechanic
{
    public void _()
    {
        var or = OrderPoint.Instance;

        or.LineRenderer = or.RequireComponent<LineRenderer>();
        or.MeshRenderer = or.RequireComponent<MeshRenderer>();
            
        or.LineRenderer.positionCount = 2;
        or.LineRenderer.enabled = false;

        or.MeshRenderer.enabled = false;
    }
}