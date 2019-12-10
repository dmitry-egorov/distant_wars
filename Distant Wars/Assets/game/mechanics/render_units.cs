using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class render_units: MassiveMechanic
{
    public render_units()
    {
        sprite_vertices = new List<Vector3>(0);
        sprite_triangles = new List<int>(0);
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
            /* delta time         */ var dt  = Time.deltaTime;
            /* blinking hide time */ var bht = ur.DamageBlinkHideTime;
            /* blinking show time */ var bst = ur.DamageBlinkShowTime;
            /* blinking period    */ var bp  = bht + bst;
            /* sprites mesh       */ var sm  = ur.SpritesMesh;
            var sv = sprite_vertices;
            var st = sprite_triangles;

            var /* sprites count */ sc = ouc + otc;

            sv.Clear();
            st.Clear();

            sv.ReserveMemoryFor(sc * 4);
            st.ReserveMemoryFor(sc * 6);

            /* sprite index */ var qi = 0;
            for (var i = 0; i < sc; i++)
            {
                /* unit                   */ var u   = i < ouc ? ous[i] : otu[i - ouc];
                /* original blinking time */ var obt = u.BlinkTimeRemaining;
                /* updated  blinking time */ var ubt = u.BlinkTimeRemaining = obt > 0 ? obt - dt : 0;
                // hide, when blinking and in hiding period
                if (ubt > 0 && (ubt % bp) > bst)
                {
                    continue;
                }

                /* unit is selected       */ var uih = u.IsHighlighted;
                /* unit is selected       */ var uis = u.IsSelected;
                /* faction color index    */ var fci = u.Faction.Index;
                /* highlight flags        */ var hf  = (uih ? 1 : 0) | (uis ? 2 : 0) | (fci << 4);

                for (var j = 0; j < 4; j++)
                {
                    // flags for the vertex shader, right to left: is highlighted (1 bit), is selected (1 bit), quad index (2 bits), color index (4 bits) 
                    var fl = hf | j << 2;
                    var p = u.Position;
                    sv.Add(p.xy(fl));
                }

                //PERF: since the quads are not changing, we should only generate them when capacity is increasing.
                RenderHelper.add_quad(st, qi);
                qi++;
            }

            sm.Clear();
            sm.SetVertices(sv);
            sm.SetTriangles(st, 0, false);
        }

        // generate faction colors
        {
            var fs  = Faction.Factions;
            var fsc = fs.Count;
            var mfc = MaxNumberOfFactionColors;
            var cs  = fsc;

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

        Shader.SetGlobalFloat(units_size_id, ur.SpriteSize * Screen.height / 1080);
    }

    readonly List<Vector3> sprite_vertices;
    readonly List<int> sprite_triangles;
    readonly Vector4[] faction_colors;
    
    const int MaxNumberOfFactionColors = 16;
    static readonly int units_size_id = Shader.PropertyToID("_UnitsSize");
    static readonly int faction_colors_id = Shader.PropertyToID("_FactionColors");

}