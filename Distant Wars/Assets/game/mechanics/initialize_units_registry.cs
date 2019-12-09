using Plugins.Lanski.Behaviours;
using Plugins.Lanski.Space;
using UnityEngine;

internal class initialize_units_registry : MassiveMechanic
{
    public void _()
    {
        /* units registry */ var ur = UnitsRegistry.Instance;
        /* map            */ var map = Map.Instance;

        var sr = ur.SpritesRenderer;
        var vr = ur.VisionRenderer;
        var dr = ur.DiscoveryRenderer;

        var vmf = vr.RequireComponent<MeshFilter>();
        var dmf = dr.RequireComponent<MeshFilter>();
        var smf = sr.RequireComponent<MeshFilter>();
        var vmesh = vmf.sharedMesh;
        var dmesh = dmf.sharedMesh;
        var smesh = smf.sharedMesh;

        if (vmesh == null) vmesh = vmf.sharedMesh = new Mesh {name = "vision quads"};
        if (dmesh == null) dmesh = dmf.sharedMesh = new Mesh {name = "discovery quads"};
        if (smesh == null) smesh = smf.sharedMesh = new Mesh {name = "unit sprites"};

        vmesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
        dmesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
        smesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
        dmesh.MarkDynamic();
        dmesh.MarkDynamic();

        smesh.MarkDynamic();

        ur.VisionMesh = vmesh;
        ur.DiscoveryMesh = dmesh;
        ur.SpritesMesh = smesh;

        ur.Units.Clear();
        ur.VisionUnits.Clear();
        ur.OtherUnits.Clear();
    }
}