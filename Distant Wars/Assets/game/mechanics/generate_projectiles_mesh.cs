using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class generate_projectiles_mesh : MassiveMechanic
{
    public generate_projectiles_mesh()
    {
        sprite_vertices = new List<Vector3>(0);
        sprite_triangles = new List<int>(0);
    }

    public void _()
    {
        /* units' registry */ var pm = ProjectilesManager.Instance;

        // generate sprites
        var sm = pm.SpritesMesh;
        var sv = sprite_vertices;
        var st = sprite_triangles;

        var /* positions     */ ps = pm.Positions;
        var /* sprites count */ sc = ps.Count;

        sv.Clear();
        st.Clear();

        sv.ReserveMemoryFor(sc * 4);
        st.ReserveMemoryFor(sc * 6);

        for (var i = 0; i < sc; i++)
        {
            var /* position */ p = ps[i].xy();
            
            for (var j = 0; j < 4; j++)
            {
                sv.Add(p.xy(j));
            }

            //PERF: since the quads are not changing, we should only generate them when capacity is increasing.
            RenderHelper.add_quad(st, i);
        }

        sm.Clear();
        sm.SetVertices(sv);
        sm.SetTriangles(st, 0, false);

        Shader.SetGlobalFloat(bullets_size_id, pm.SpriteSize * Screen.height / 1080);
    }
    
    private List<Vector3> sprite_vertices;
    private List<int> sprite_triangles;

    static readonly int bullets_size_id = Shader.PropertyToID("_BulletsSize");

}