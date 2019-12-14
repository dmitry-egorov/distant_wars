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

        var time_ratio = Game.Instance.PresentationToSimulationFrameTimeRatio;

        // generate sprites
        var sm = pm.SpritesMesh;
        var sv = sprite_vertices;
        var st = sprite_triangles;

        /* positions     */ var ps  = pm.positions;
        /* positions     */ var pps = pm.prev_positions;
        /* sprites count */ var sc  = ps.Count;

        sv.Clear();
        st.Clear();

        sv.ReserveMemoryFor(sc * 4);
        st.ReserveMemoryFor(sc * 6);

        for (var i = 0; i < sc; i++)
        {
            /* position      */ var p  = ps [i].xy();
            /* prev position */ var pp = pps[i];
            /* interpolated position */ var ip = Vector2.Lerp(pp, p, time_ratio);
            
            for (var j = 0; j < 4; j++)
            {
                sv.Add(ip.xy(j));
            }

            //PERF: since the quads are not changing, we should only generate them when capacity is increasing.
            RenderHelper.add_quad(st, i);
        }

        sm.Clear();
        sm.SetVertices(sv);
        sm.SetTriangles(st, 0, false);

        Shader.SetGlobalFloat(bullets_size_id, pm.SpriteSize * (Screen.height / 1080));
    }
    
    private List<Vector3> sprite_vertices;
    private List<int> sprite_triangles;

    static readonly int bullets_size_id = Shader.PropertyToID("_BulletsSize");

}