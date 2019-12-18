using System.Collections.Generic;
using Plugins.Lanski;
using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class init_projectiles_manager : MassiveMechanic
{
    public void _()
    {
        var map = Map.Instance;
        var  pm = ProjectilesManager.Instance;
        pm.shooters       = new List<Unit>();
        pm.positions      = new LeakyList<Vector3>();
        pm.prev_positions = new LeakyList<Vector2>();
        pm.directions     = new LeakyList<Vector3>();
        pm.speeds         = new LeakyList<float>();
        pm.damages        = new LeakyList<int>();

        /* mesh renderer */ var sr = pm.SpritesRenderer;

        var smf = sr.RequireComponent<MeshFilter>();
        var smesh = smf.sharedMesh;

        if (smesh == null) smesh = smf.sharedMesh = new Mesh { name = "projectile sprites" };
    
        smesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
        smesh.MarkDynamic();

        pm.SpritesMesh = smesh;
    }
}