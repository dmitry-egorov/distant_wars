using System.Collections.Generic;
using UnityEngine;

public class render_units: MassiveMechanic
{
    public void _()
    {
        var /* units' registry */ ur = UnitsRegistry.Instance;
        var /* units */ us = ur.Units;
        
        if (vertices == null) vertices = new List<Vector3>(us.Count * 4);
        if (triangles == null) triangles = new List<int>(us.Count * 6);
        
        var /* mesh */ m = ur.Mesh;
        
        vertices.Clear();
        triangles.Clear();
        
        for (var i = 0; i < us.Count; i++)
        {
            var /* unit */ u = us[i];
            var /* highlight flags */ hf = 
                  (u.IsHighlighted ? 1 : 0)
                | (u.IsSelected ? 2 : 0)
            ;

            for (var j = 0; j < 4; j++)
            {
                var /* flags */ f = hf | j << 2;
                vertices.Add(u.Position.xy(f));
            }

            triangles.Add(i * 4 + 0);
            triangles.Add(i * 4 + 1);
            triangles.Add(i * 4 + 2);
            triangles.Add(i * 4 + 1);
            triangles.Add(i * 4 + 3);
            triangles.Add(i * 4 + 2);
        }

        m.SetVertices(vertices);
        m.SetTriangles(triangles, 0, false);
        
        ur.Material.SetFloat(size, ur.UnitScreenSize * 32 * (Screen.height / 1080));

        ur.MeshRenderer.enabled = true;
    }

    List<Vector3> vertices;
    List<int> triangles;
    
    static readonly int size = Shader.PropertyToID("_Size");
}