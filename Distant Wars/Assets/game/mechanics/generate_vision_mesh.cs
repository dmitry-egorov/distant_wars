using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

internal class generate_vision_mesh : MassiveMechanic
{
    public generate_vision_mesh()
    {
        vision_vertices = new List<Vector3>(0);
        vision_triangles = new List<int>(0);
        discovery_vertices = new List<Vector3>(0);
        discovery_triangles = new List<int>(0);
    }
    
    public void _()
    {
        var /* units' registry  */ ur  = UnitsRegistry.Instance;
        var /* vision mesh      */ vm  = ur.VisionMesh;
        var /* vision mesh      */ dm  = ur.DiscoveryMesh;
        var /* units            */ ous = ur.OwnTeamUnits;
        var /* own unit's count */ ouc = ous.Count;

        /* strategic camera */ var sc  = StrategicCamera.Instance;
        /* screen rect in world space */ var ws = sc.WorldScreen;

        var vv = vision_vertices;
        var vt = vision_triangles;
        var dv = discovery_vertices;
        var dt = discovery_triangles;

        var /* visions count */ vc = ouc;
        vv.Clear();
        vt.Clear();
        dv.Clear();
        dt.Clear();
        vv.ReserveMemoryFor(vc * 4);
        vt.ReserveMemoryFor(vc * 6);
        dv.ReserveMemoryFor(vc * 4);
        dt.ReserveMemoryFor(vc * 6);

        /* quad index */ var qi = 0;
        for (var i = 0; i < vc; i++)
        {
            var /* unit        */  u = ous[i];
            var /* vision size */ vs = u.VisionRange * 2.0f;

            var p = u.Position;
            var qmin = new Vector2(p.x - 0.5f * vs, p.y - 0.5f * vs);
            var qmax = new Vector2(p.x + 0.5f * vs, p.y + 0.5f * vs);

            var tl = new Vector3(p.x - 0.5f * vs, p.y + 0.5f * vs, 0);
            var br = new Vector3(p.x + 0.5f * vs, p.y - 0.5f * vs, 3);
            var tr = qmax.xy(1);
            var bl = qmin.xy(2);

            // is within the screen rect
            if (ws.intersects(qmin, qmax))
            {
                vv.Add(tl);
                vv.Add(tr);
                vv.Add(bl);
                vv.Add(br);

                RenderHelper.add_quad(vt, qi);
                qi++;
            }

            dv.Add(tl);
            dv.Add(tr);
            dv.Add(bl);
            dv.Add(br);

            RenderHelper.add_quad(dt, i);
        }

        vm.Clear();
        vm.SetVertices(vv);
        vm.SetTriangles(vt, 0, false);

        dm.Clear();
        dm.SetVertices(dv);
        dm.SetTriangles(dt, 0, false);

        DebugText.set_text("vision quads", $"{qi}");
    }

    readonly List<Vector3> vision_vertices;
    readonly List<int> vision_triangles;
    readonly List<Vector3> discovery_vertices;
    readonly List<int> discovery_triangles;
}