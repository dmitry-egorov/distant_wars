using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class initialize_units_registry : MassiveMechanic
{
    public void _()
    {
        var /* units registry */  ur = UnitsRegistry.Instance;
        var map = Map.Instance;

        {
            var /* mesh renderer  */ sr = ur.SpritesRenderer;
            var /* material       */ sm = sr.sharedMaterial;

            var smf = sr.RequireComponent<MeshFilter>();
            var smesh = smf.sharedMesh;

            if (smesh == null) 
                smesh = smf.sharedMesh = new Mesh {name = "unit sprites"};
        
            smesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
            smesh.MarkDynamic();

            ur.SpritesMesh = smesh;
        }

        {
            var /* mesh renderer  */ vr = ur.VisionRenderer;
            var /* material       */ vm = vr.sharedMaterial;
        
            var vmf = vr.RequireComponent<MeshFilter>();
            var vmesh = vmf.sharedMesh;

            if (vmesh == null) 
                vmesh = vmf.sharedMesh = new Mesh {name = "vision quads"};
        
            vmesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
            vmesh.MarkDynamic();

            ur.VisionMesh = vmesh;
        }
    }
}