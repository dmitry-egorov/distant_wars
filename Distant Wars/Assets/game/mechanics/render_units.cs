using System.Collections.Generic;
using UnityEngine;

public class render_units: MassiveMechanic
{
    public void _()
    {
        var /* units' registry */ ur = UnitsRegistry.Instance;
        if (ur.SingleMesh)
        {
            var /* units */ us = ur.Units;
            
            if (vertices == null) vertices = new List<Vector3>(us.Count * 4);
            if (triangles == null) triangles = new List<int>(us.Count * 6);
            
            foreach (var u in us)
                u.SpriteRenderer.enabled = false;

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
                
                /*
                var vp = u.Position;
                
                vertices.Add(new Vector3(vp.x - 0.5f, vp.y + 0.5f, hf));
                vertices.Add(new Vector3(vp.x + 0.5f, vp.y + 0.5f, hf));
                vertices.Add(new Vector3(vp.x - 0.5f, vp.y - 0.5f, hf));
                vertices.Add(new Vector3(vp.x + 0.5f, vp.y - 0.5f, hf));
                */

                triangles.Add(i * 4 + 0);
                triangles.Add(i * 4 + 1);
                triangles.Add(i * 4 + 2);
                triangles.Add(i * 4 + 1);
                triangles.Add(i * 4 + 3);
                triangles.Add(i * 4 + 2);
            }

            m.SetVertices(vertices);
            m.SetTriangles(triangles, 0, false);
            //mesh.UploadMeshData(false);
            
            ur.Material.SetFloat(size, ur.UnitScreenSize * 32);

            ur.MeshRenderer.enabled = true;
        }
        else // individual mesh sprite renderers
        {
            var /* strategic camera */  sc = StrategicCamera.Instance;
            var            /* units */  us = ur.Units;
            var     /* unit's scale */   s = ur.WorldScale;
            var     /* scale vector */ scv = s * Vector3.one;
            var         /* unit's z */   z = UnitsRegistry.Instance.Z;
        
            var /* world to screen  multiplier */ m = sc.WorldToScreenSpaceMultiplier;
            var      /* world to screen offset */ o = sc.WorldToScreenSpaceOffset;

            if (Application.isPlaying)
            {
                foreach (var /* unit */ u in us)
                {
                    var /* position */ p = u.Position;

                    var  /* screen position */  sp = p * m + o;
                    var /* snapped position */ snp = (sp.Floor() - o) / m;

                    var ut = u.transform;
                    ut.localScale = scv;
                    ut.position = snp.xy(z + u.BaseZOffset + u.StyleZOffset);
                    u.SpriteRenderer.enabled = true;
                }
            }
            else
            {
                foreach (var /* unit */ u in us)
                {
                    var ut = u.transform;
                    ut.localScale = scv;
                    u.SpriteRenderer.enabled = true;
                }
            }
            
            ur.MeshRenderer.enabled = false;
        }
    }

    List<Vector3> vertices;
    List<int> triangles;
    
    static readonly int size = Shader.PropertyToID("_Size");
}