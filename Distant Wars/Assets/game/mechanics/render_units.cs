using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class render_units: MassiveMechanic
{
    public render_units()
    {
        vision_vertices = new List<Vector3>(0);
        sprite_vertices = new List<Vector3>(0);
        sprite_triangles = new List<int>(0);
        vision_triangles = new List<int>(0);
    }
    
    public void _()
    {
        var /* units' registry */  ur = UnitsRegistry.Instance;
        var /* units           */ ous = ur.OwnUnits;
        var /* units           */ otu = ur.OtherVisibleUnits;
        var /* sprites mesh    */  sm = ur.SpritesMesh;
        var /* sprites mesh    */  vm = ur.VisionMesh;

        var sv = sprite_vertices;
        var st = sprite_triangles;
        var vv = vision_vertices;
        var vt = vision_triangles;

        sv.Clear();
        st.Clear();
        vv.Clear();
        vt.Clear();

        var /* own unit's count   */ ouc = ous.Count;
        var /* other unit's count */ otc = otu.Count;

        var /* sprites count */ sc = ouc + otc;
        var /* visions count */ vc = ouc;

        sv.ReserveMemoryFor(sc * 4);
        st.ReserveMemoryFor(sc * 6);
        vv.ReserveMemoryFor(vc * 4);
        vt.ReserveMemoryFor(vc * 6);
        
        // generate sprites
        for (var i = 0; i < sc; i++)
        {
            var /* unit             */   u = i < ouc ? ous[i] : otu[i - ouc];
            var /* unit is selected */ uih = u.IsHighlighted;
            var /* unit is selected */ uis = u.IsSelected;
            var /* highlight flags  */  hf = (uih ? 1 : 0) | (uis ? 2 : 0);

            for (var j = 0; j < 4; j++)
            {
                // flags for the vertex shader, right to left: is highlighted (1 bit), is selected (1 bit), quad index (2 bits) 
                var fl = hf | j << 2;
                var p = u.Position;
                sv.Add(p.xy(fl));
            }

            //TODO: we should only generate additional triangles when capacity is increasing, since they're not changing
            add_quad(st, i);
        }
        
        // generate visions
        for (var i = 0; i < vc; i++)
        {
            var /* unit        */  u = ous[i];
            var /* vision size */ vs = u.VisionRange * 2.0f;

            for (var j = 0; j < 4; j++)
            {
                var p = u.Position;
                vv.Add((p + vs * (j % 2 - 0.5f) * Vector2.right + vs * (j / 2 - 0.5f) * Vector2.down).xy(j));
            }

            //TODO: we should only generate additional triangles when capacity is increasing, since they're not changing
            add_quad(vt, i);
        }

        sm.Clear();
        sm.SetVertices(sv);
        sm.SetTriangles(st, 0, false);

        vm.Clear();
        vm.SetVertices(vv);
        vm.SetTriangles(vt, 0, false);

        Shader.SetGlobalFloat(units_size, ur.UnitScreenSize * 32 * (Screen.height / 1080));
    }

    private static void add_quad(List<int> vt, int i)
    {
        vt.Add(i * 4 + 0);
        vt.Add(i * 4 + 1);
        vt.Add(i * 4 + 2);
        vt.Add(i * 4 + 1);
        vt.Add(i * 4 + 3);
        vt.Add(i * 4 + 2);
    }

    readonly List<Vector3> sprite_vertices;
    readonly List<Vector3> vision_vertices;
    readonly List<int> sprite_triangles;
    readonly List<int> vision_triangles;
    
    static readonly int units_size = Shader.PropertyToID("_UnitsSize");
}