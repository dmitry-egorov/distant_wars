using Plugins.Lanski;
using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class init_explosions_manager : IMassiveMechanic
{
    public void _()
    {
        var em = ExplosionsManager.Instance;
        em.positions = new LeakyList<Vector3>(1024);
        em.remaining_times = new LeakyList<float>(1024);
        var sr = em.SpritesRenderer;
        var map = Map.Instance;

        var smf = sr.RequireComponent<MeshFilter>();
        var smesh = smf.sharedMesh;

        if (smesh == null) smesh = smf.sharedMesh = new Mesh { name = "explosion sprites" };
    
        smesh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
        smesh.MarkDynamic();

        em.SpritesMesh = smesh;
    }
}