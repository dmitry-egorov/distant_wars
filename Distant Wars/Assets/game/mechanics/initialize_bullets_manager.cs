using System.Collections.Generic;
using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class initialize_bullets_manager : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        var  pm = ProjectilesManager.Instance;
        pm.Positions  = new List<Vector3>();
        pm.Directions = new List<Vector3>();
        pm.Speeds  = new List<float>();
        pm.Damages = new List<float>();

        /* mesh renderer */ var sr = pm.SpritesRenderer;

        var smf = sr.RequireComponent<MeshFilter>();
        var smesh = smf.sharedMesh;

        if (smesh == null) 
            smesh = smf.sharedMesh = new Mesh {name = "projectile sprites"};
    
        smesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
        smesh.MarkDynamic();

        pm.SpritesMesh = smesh;
    }
}