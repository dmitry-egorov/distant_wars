using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class render_units: MassiveMechanic
{
    public render_units()
    {
        vision_vertices = new List<Vector3>(0);
        sprite_vertices = new List<Vector3>(0);
        triangles = new List<int>(0);
    }
    
    public void _()
    {
        var /* units' registry */ ur = UnitsRegistry.Instance;
        var /* units           */ us = ur.Units;
        var /* sprites mesh    */ sm = ur.SpritesMesh;
        var /* sprites mesh    */ vm = ur.VisionMesh;

        var sv = sprite_vertices;
        var vv = vision_vertices;
        var ts = triangles;
        
        sv.Clear();
        vv.Clear();
        ts.Clear();

        var uc = us.Count;
        sv.ReserveMemoryFor(uc * 4);
        vv.ReserveMemoryFor(uc * 4);
        ts.ReserveMemoryFor(uc * 6);

        for (var i = 0; i < uc; i++)
        {
            var /* unit             */   u = us[i];
            var /* unit is selected */ uih = u.IsHighlighted;
            var /* unit is selected */ uis = u.IsSelected;
            var /* highlight flags  */  hf = (uih ? 1 : 0) | (uis ? 2 : 0);
            var /* vision range     */  vr = u.VisionRange; 

            for (var j = 0; j < 4; j++)
            {
                // flags for the vertex shader, right to left: is highlighted (1 bit), is selected (1 bit), quad index (2 bits) 
                var fl = hf | j << 2;
                var p = u.Position;
                sv.Add(p.xy(fl));
                vv.Add((p + vr * (j % 2 - 0.5f) * Vector2.right + vr * (j / 2 - 0.5f) * Vector2.down).xy(j));
            }

            //TODO: we should only generate additional triangles when capacity is increasing, since they're not changing
            ts.Add(i * 4 + 0);
            ts.Add(i * 4 + 1);
            ts.Add(i * 4 + 2);
            ts.Add(i * 4 + 1);
            ts.Add(i * 4 + 3);
            ts.Add(i * 4 + 2);
        }

        sm.SetVertices(sv);
        sm.SetTriangles(ts, 0, false);
        vm.SetVertices(vv);
        vm.SetTriangles(ts, 0, false);
        
        Shader.SetGlobalFloat(units_size, ur.UnitScreenSize * 32 * (Screen.height / 1080));

        ur.SpritesRenderer.enabled = true;
        ur.VisionRenderer.enabled = true;
        
    }

    readonly List<Vector3> sprite_vertices;
    readonly List<Vector3> vision_vertices;
    readonly List<int> triangles;
    
    static readonly int units_size = Shader.PropertyToID("_UnitsSize");
}