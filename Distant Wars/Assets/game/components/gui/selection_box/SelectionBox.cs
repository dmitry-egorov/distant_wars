using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MeshRenderer))]
public class SelectionBox : RequiredSingleton<SelectionBox>
{
    public void enable(Rect b)
    {
        var /* position */  p = b.center;
        var /* scale */     s = b.size;
        var /* transform */ t = transform;
        t.position = p.xy(t.position.z);
        t.localScale = s.xy1();
        Renderer.enabled = true;
    }

    public void disable()
    {
        Renderer.enabled = false;
    }

    MeshRenderer Renderer
    {
        get
        {
            if (mesh_renderer != null)
                return mesh_renderer;
            
            mesh_renderer = GetComponent<MeshRenderer>();
            Assert.IsNotNull(mesh_renderer);
            return mesh_renderer;
        }
    }

    MeshRenderer mesh_renderer;
}