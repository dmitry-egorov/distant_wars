using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

internal class generate_explosions_mesh : IMassiveMechanic
{
    public generate_explosions_mesh()
    {
        sprite_vertices = new List<Vector3>(0);
        sprite_triangles = new List<int>(0);
    }

    public void _()
    {
        /* units' registry */ var em = ExplosionsManager.Instance;

        // generate sprites
        var sm = em.SpritesMesh;
        var vs = sprite_vertices;
        var st = sprite_triangles;

        /* positions     */ var ps  = em.positions;
        /* positions     */ var rts = em.remaining_times;
        /* sprites count */ var sc  = ps.Count;
        var size = em.ExplosionSize;
        var edur = em.ExplosionDuration;

        vs.Clear();
        st.Clear();

        vs.ReserveMemoryFor(sc * 4);
        st.ReserveMemoryFor(sc * 6);

        for (var i = 0; i < sc; i++)
        {
            /* position */ var p  = ps [i].xy();
            var t = 1 - (rts[i] / edur);
            
            var tl = new Vector3(p.x - size, p.y + size, 0 + t);
            var tr = new Vector3(p.x + size, p.y + size, 1 + t);
            var bl = new Vector3(p.x - size, p.y - size, 2 + t);
            var br = new Vector3(p.x + size, p.y - size, 3 + t);
            
            vs.Add(tl);
            vs.Add(tr);
            vs.Add(bl);
            vs.Add(br);

            RenderHelper.add_quad(st, i);
        }

        sm.Clear();
        sm.SetVertices(vs);
        sm.SetTriangles(st, 0, false);
    }
    
    private readonly List<Vector3> sprite_vertices;
    private readonly List<int> sprite_triangles;
}
