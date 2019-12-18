using System.Collections.Generic;
using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class init_units_registry : MassiveMechanic
{
    public void _()
    {
        /* units registry */ var ur  = UnitsRegistry.Instance;
        /* map            */ var map = Map.Instance;

        var bounds = new Bounds(Vector3.zero, new Vector3(3.0f * map.Scale, 3.0f * map.Scale, 1));

        ur.SpritesMesh          = init_renderer(ur.SpritesRenderer,          "unit sprites");
        ur.HPSpritesMesh        = init_renderer(ur.HPSpritesRenderer,        "hp sprites");
        ur.VisionCirclesMesh    = init_renderer(ur.VisionCirclesRenderer,    "vision circles");
        ur.VisionQuadsMesh      = init_renderer(ur.VisionQuadsRenderer,      "vision quads");
        ur.DiscoveryCirclesMesh = init_renderer(ur.DiscoveryCirclesRenderer, "discovery circles");;
        ur.DiscoveryQuadsMesh   = init_renderer(ur.DiscoveryQuadsRenderer,   "discovery quads");;;

        ur.Units = new List<Unit>();
        ur.OwnTeamUnits = new List<Unit>();

        Mesh init_renderer(MeshRenderer mr, string name)
        {
            var mf = mr.RequireComponent<MeshFilter>();
            var mesh = mf.sharedMesh;

            if (mesh == null) mesh = mf.sharedMesh = new Mesh { name = name };

            mesh.bounds = bounds;
            mesh.MarkDynamic();
            return mesh;
        }
    }
}