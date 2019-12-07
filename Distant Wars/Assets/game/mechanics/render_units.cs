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
        faction_colors = new Vector4[MaxNumberOfFactionColors];
    }
    
    public void _()
    {
        var /* units' registry */  ur = UnitsRegistry.Instance;
        var /* units           */ ous = ur.VisionUnits;
        var /* units           */ otu = ur.VisibleOtherUnits;

        var /* own unit's count   */ ouc = ous.Count;
        var /* other unit's count */ otc = otu.Count;
        
        // generate sprites
        {
            var /* sprites mesh */  sm = ur.SpritesMesh;
            var sv = sprite_vertices;
            var st = sprite_triangles;

            var /* sprites count */ sc = ouc + otc;

            sv.Clear();
            st.Clear();

            sv.ReserveMemoryFor(sc * 4);
            st.ReserveMemoryFor(sc * 6);

            for (var i = 0; i < sc; i++)
            {
                var /* unit             */      u = i < ouc ? ous[i] : otu[i - ouc];
                var /* unit is selected */    uih = u.IsHighlighted;
                var /* unit is selected */    uis = u.IsSelected;
                var /* faction color index */ fci = u.Faction.Index;
                var /* highlight flags  */     hf = (uih ? 1 : 0) | (uis ? 2 : 0) | (fci << 4);

                for (var j = 0; j < 4; j++)
                {
                    // flags for the vertex shader, right to left: is highlighted (1 bit), is selected (1 bit), quad index (2 bits), color index (4 bits) 
                    var fl = hf | j << 2;
                    var p = u.Position;
                    sv.Add(p.xy(fl));
                }

                //TODO: since the quads are not changing, we should only generate additional them when capacity is increasing.
                RenderHelper.add_quad(st, i);
            }

            sm.Clear();
            sm.SetVertices(sv);
            sm.SetTriangles(st, 0, false);
        }

        // generate vision
        {
            var /* vision mesh */ vm = ur.VisionMesh;
            var vv = vision_vertices;
            var vt = vision_triangles;

            var /* visions count */ vc = ouc;
            vv.Clear();
            vt.Clear();
            vv.ReserveMemoryFor(vc * 4);
            vt.ReserveMemoryFor(vc * 6);

            for (var i = 0; i < vc; i++)
            {
                var /* unit        */  u = ous[i];
                var /* vision size */ vs = u.VisionRange * 2.0f;

                for (var j = 0; j < 4; j++)
                {
                    var p = u.Position;
                    vv.Add((p + vs * (j % 2 - 0.5f) * Vector2.right + vs * (j / 2 - 0.5f) * Vector2.down).xy(j));
                }

                //TODO: since the quads are not changing, we should only generate additional them when capacity is increasing.
                RenderHelper.add_quad(vt, i);
            }
            
            vm.Clear();
            vm.SetVertices(vv);
            vm.SetTriangles(vt, 0, false);
        }

        // generate faction colors
        {
            var fs = Faction.Factions;
            var fsc = fs.Count;
            var mfc = MaxNumberOfFactionColors;
            var cs = fsc;

            if (fsc > mfc)
            {
                cs = mfc;
                DebugText.set_text("Exceeded maximum allowed number of factions", fs.Count.ToString());
            }

            for (var i = 0; i < cs; i++)
            {
                faction_colors[i] = fs[i].Color;
            }

            Shader.SetGlobalVectorArray(faction_colors_id, faction_colors);
        }

        Shader.SetGlobalFloat(units_size_id, ur.UnitScreenSize * 32 * (Screen.height / 1080));
    }

    readonly List<Vector3> sprite_vertices;
    readonly List<Vector3> vision_vertices;
    readonly List<int> sprite_triangles;
    readonly List<int> vision_triangles;
    readonly Vector4[] faction_colors;
    
    const int MaxNumberOfFactionColors = 16;
    static readonly int units_size_id = Shader.PropertyToID("_UnitsSize");
    static readonly int faction_colors_id = Shader.PropertyToID("_FactionColors");

}