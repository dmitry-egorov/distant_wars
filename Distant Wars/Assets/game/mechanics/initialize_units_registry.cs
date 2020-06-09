using System.Collections.Generic;
using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class init_units_registry : IMassiveMechanic
{
    public void _()
    {
        /* units registry */ var ur  = UnitsRegistry.Instance;
        /* map            */ var map = Map.Instance;

        var bounds = new Bounds(Vector3.zero, new Vector3(3.0f * map.Scale, 3.0f * map.Scale, 1));

        ur.sprites_mesh           = init_renderer(ur.SpritesRenderer,          "unit sprites");
        ur.hp_sprites_mesh        = init_renderer(ur.HPSpritesRenderer,        "hp sprites");
        ur.vision_circles_mesh    = init_renderer(ur.VisionCirclesRenderer,    "vision circles");
        ur.vision_quads_mesh      = init_renderer(ur.VisionQuadsRenderer,      "vision quads");
        ur.discovery_circles_mesh = init_renderer(ur.DiscoveryCirclesRenderer, "discovery circles");;
        ur.discovery_quads_mesh   = init_renderer(ur.DiscoveryQuadsRenderer,   "discovery quads");;;

        ur.all_units = new List<Unit>();
        ur.local_team_units = new List<Unit>();

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