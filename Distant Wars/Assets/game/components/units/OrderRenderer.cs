using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class OrderRenderer : MonoBehaviour
{
    public void init()
    {
        Assert.IsNotNull(line_renderer = GetComponent<LineRenderer>());
        Assert.IsNotNull(mesh_renderer = GetComponent<MeshRenderer>());

        line_renderer.positionCount = 2;
        line_renderer.enabled = false;

        mesh_renderer.enabled = false;
    }

    public void show_move_order(Vector2 /* position */ p, Vector2 /* target */ t)
    {
        show_move_order_line(p, t);
        show_move_order_point(t);
    }

    void show_move_order_point(Vector2 t)
    {
        transform.position = t.xy(UnitsRegistry.Instance.OrdersZ);
        
        mesh_renderer.enabled = true;
    }

    void show_move_order_line(Vector2 p, Vector2 t)
    {
        var sc = StrategicCamera.Instance;
        
        var z = UnitsRegistry.Instance.OrdersZ;
        line_renderer.SetPosition(0, p.xy(z));
        line_renderer.SetPosition(1, t.xy(z));

        var w = UnitsRegistry.Instance.OrderLineWidth * sc.SizeProportion;
        line_renderer.startWidth = w;
        line_renderer.endWidth = w;

        line_renderer.enabled = true;
    }

    public void hide()
    {
        line_renderer.enabled = false;
        mesh_renderer.enabled = false;
    }
    
    LineRenderer line_renderer;
    MeshRenderer mesh_renderer;
}
