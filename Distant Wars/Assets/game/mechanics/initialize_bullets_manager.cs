using System.Collections.Generic;
using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class initialize_bullets_manager : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        var bm = BulletsManager.Instance;
        bm.Positions = new List<Vector2>();
        bm.Velocities = new List<Vector2>();
        bm.Damages = new List<float>();

        var /* mesh renderer */ sr = bm.SpritesRenderer;

        var smf = sr.RequireComponent<MeshFilter>();
        var smesh = smf.sharedMesh;

        if (smesh == null) 
            smesh = smf.sharedMesh = new Mesh {name = "bullet sprites"};
    
        smesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
        smesh.MarkDynamic();

        bm.SpritesMesh = smesh;
    }
}