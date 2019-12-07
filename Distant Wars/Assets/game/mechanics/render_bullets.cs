using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class render_bullets : MassiveMechanic
{
    public render_bullets()
    {
        sprite_vertices = new List<Vector3>(0);
        sprite_triangles = new List<int>(0);
    }

    public void _()
    {
        const float /*bullet size */ bs = 2f;
        var /* units' registry */  bm = BulletsManager.Instance;

        // generate sprites
        var /* sprites mesh */ sm = bm.SpritesMesh;
        var sv = sprite_vertices;
        var st = sprite_triangles;

        var /* positions     */ ps = bm.Positions;
        var /* sprites count */ sc = ps.Count;

        sv.Clear();
        st.Clear();

        sv.ReserveMemoryFor(sc * 4);
        st.ReserveMemoryFor(sc * 6);

        for (var i = 0; i < sc; i++)
        {
            var /* unit */ p = ps[i];
            
            for (var j = 0; j < 4; j++)
            {
                sv.Add((p + bs * (j % 2 - 0.5f) * Vector2.right + bs * (j / 2 - 0.5f) * Vector2.down).xy(j));
            }

            //TODO: since the quads are not changing, we should only generate additional them when capacity is increasing.
            RenderHelper.add_quad(st, i);
        }

        sm.Clear();
        sm.SetVertices(sv);
        sm.SetTriangles(st, 0, false);
    }
    
    private List<Vector3> sprite_vertices;
    private List<int> sprite_triangles;
}