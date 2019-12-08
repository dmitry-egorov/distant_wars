using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class initialize_units_registry : MassiveMechanic
{
    public void _()
    {
        /* units registry */ var ur = UnitsRegistry.Instance;
        var map = Map.Instance;

        {
            var /* mesh renderer  */ sr = ur.SpritesRenderer;

            var smf = sr.RequireComponent<MeshFilter>();
            var smesh = smf.sharedMesh;

            if (smesh == null) 
                smesh = smf.sharedMesh = new Mesh {name = "unit sprites"};
        
            smesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
            smesh.MarkDynamic();

            ur.SpritesMesh = smesh;
        }

        {
            var vr = ur.VisionRenderer;
            var dr = ur.DiscoveryRenderer;

            var vmf = vr.RequireComponent<MeshFilter>();
            var dmf = dr.RequireComponent<MeshFilter>();
            var vmesh = vmf.sharedMesh;

            if (vmesh == null)
                vmesh = dmf.sharedMesh = vmf.sharedMesh = new Mesh {name = "vision quads"};
        
            vmesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
            vmesh.MarkDynamic();

            ur.VisionMesh = vmesh;
        }

        ur.Units.Clear();
        ur.VisionUnits.Clear();
        ur.OtherUnits.Clear();
    }
}