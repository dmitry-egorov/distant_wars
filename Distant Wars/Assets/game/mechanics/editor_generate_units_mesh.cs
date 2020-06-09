using System.Collections.Generic;
using UnityEngine;

public class editor_generate_units_mesh : MassiveMechanic
{
    public editor_generate_units_mesh()
    {
        sprite_vertices = new List<Vector3>(0);
        sprite_triangles = new List<int>(0);
    }

    public void _()
    {
        var ur = UnitsRegistry.Instance;
        var units = ur.Units;

        var sv = sprite_vertices;
        var st = sprite_triangles;

        sv.Clear();
        st.Clear();

        for (int i = 0; i < units.Count; i++)
        {
            var unit = ur.Units[i];
            /* unit is selected    */ var uih = unit.is_highlighted;
            /* unit is selected    */ var uis = unit.is_selected;
            /* faction color index */ var fci = unit.Faction.Index;
            /* highlight flags     */ var hf  = (uih ? 1 : 0) | (uis ? 2 : 0) | (fci << 4);
            
            /* unit's interpolated position */ var uipos = unit.position;
            for (var j = 0; j < 4; j++)
            {
                // flags for the vertex shader, right to left: is highlighted (1 bit), is selected (1 bit), quad index (2 bits), color index (4 bits) 
                var fl = hf | j << 2;
                sv.Add(uipos.xy(fl));
            }

            RenderHelper.add_quad(st, i);
        }

        /* sprites mesh */ var sm  = ur.SpritesMesh;

        sm.Clear();
        sm.SetVertices(sv);
        sm.SetTriangles(st, 0, false);
    }

    readonly List<Vector3> sprite_vertices;
    readonly List<int> sprite_triangles;
    readonly Vector4[] faction_colors;
}
