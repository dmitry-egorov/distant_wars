using System.Collections.Generic;
using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class init_units_registry : MassiveMechanic
{
    public void _()
    {
        /* units registry */ var ur  = UnitsRegistry.Instance;
        /* map            */ var map = Map.Instance;

        var vcr = ur.VisionCirclesRenderer;
        var vqr = ur.VisionQuadsRenderer;
        var dcr = ur.DiscoveryCirclesRenderer;
        var dqr = ur.DiscoveryQuadsRenderer;
        var usr = ur.SpritesRenderer;

        var vcmf = vcr.RequireComponent<MeshFilter>();
        var vqmf = vqr.RequireComponent<MeshFilter>();
        var dcmf = dcr.RequireComponent<MeshFilter>();
        var dqmf = dqr.RequireComponent<MeshFilter>();
        var usmf = usr.RequireComponent<MeshFilter>();
        var vcmesh = vcmf.sharedMesh;
        var vqmesh = vqmf.sharedMesh;
        var dcmesh = dcmf.sharedMesh;
        var dqmesh = dqmf.sharedMesh;
        var usmesh = usmf.sharedMesh;

        if (vcmesh == null) vcmesh = vcmf.sharedMesh = new Mesh { name = "vision circles" };
        if (vqmesh == null) vqmesh = vqmf.sharedMesh = new Mesh { name = "vision quads" };
        if (dcmesh == null) dcmesh = dcmf.sharedMesh = new Mesh { name = "discovery circles"};
        if (dqmesh == null) dqmesh = dqmf.sharedMesh = new Mesh { name = "discovery quads"};
        if (usmesh == null) usmesh = usmf.sharedMesh = new Mesh { name = "unit sprites" };

        var bounds = new Bounds(Vector3.zero, new Vector3(3.0f * map.Scale, 3.0f * map.Scale, 1000f));
        vcmesh.bounds = bounds;
        vqmesh.bounds = bounds;
        dcmesh.bounds = bounds;
        dqmesh.bounds = bounds;
        usmesh.bounds = bounds;

        vcmesh.MarkDynamic();
        vqmesh.MarkDynamic();
        dcmesh.MarkDynamic();
        dqmesh.MarkDynamic();
        usmesh.MarkDynamic();

        ur.VisionCirclesMesh = vcmesh;
        ur.VisionQuadsMesh = vqmesh;
        ur.DiscoveryCirclesMesh = dcmesh;
        ur.DiscoveryQuadsMesh = dqmesh;
        ur.SpritesMesh = usmesh;

        ur.Units = new List<Unit>();
        ur.OwnTeamUnits = new List<Unit>();
    }
}