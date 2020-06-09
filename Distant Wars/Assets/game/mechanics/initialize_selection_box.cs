using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class init_selection_box : IMassiveMechanic
{
    public void _()
    {
        var sb = SelectionBox.Instance;
        sb.Renderer = sb.RequireComponent<MeshRenderer>();
    }
}