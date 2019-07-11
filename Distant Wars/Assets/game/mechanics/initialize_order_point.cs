using Plugins.Lanski.Behaviours;
using UnityEngine;
using UnityEngine.Assertions;

internal class initialize_order_point : MassiveMechanic
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