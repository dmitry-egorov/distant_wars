using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MassiveBehaviour<Unit>
{
    public float Speed;
    
    public Faction Faction;
    public Vector2 Position;
    
    public Vector2? MoveTarget;  

    public void issue_move_order(Vector2 /* target */ t) => MoveTarget = t;

    public void execute_order(float dt)
    {
        if (MoveTarget.TryGetValue(out var /* target */ t))
        {
            var p = Position;
            var d = (p - t).magnitude;
            var /* stopping distance */ sd = 0.5f;
            if (d > sd)
            {
                var /* speed */ s = Speed;
                var /* easing distance */ ed = 2f;
                var /* smoothed speed */  ss = s * Mathf.Clamp01(d / ed);
                Position = Vector2.MoveTowards(p, t, ss * dt);
            }
            else
            {
                MoveTarget = default;
            }
        }
    }

    public void hide_order_graphic() => order_graphic_is_visible = false;
    public void show_order_graphic() => order_graphic_is_visible = true;
    
    public void update_order_graphic()
    {
        if (order_graphic_is_visible && MoveTarget.TryGetValue(out var t))
        {
            order_renderer.show_move_order(Position, t);
        }
        else
        {
            order_renderer.hide();
        }
    }

    void Start()
    {
        if (!Application.isPlaying) return;
        
        // assert data is set
        {
            Assert.IsNotNull(Faction);
        }
        
        // find components
        {
            Assert.IsNotNull(sprite_renderer = GetComponent<SpriteRenderer>());
            Assert.IsNotNull(order_renderer = GetComponentInChildren<OrderRenderer>());
        }
        
        // apply transform to position
        {
            Position = transform.position.xy();
        }

        // apply faction material
        {
            sprite_renderer.sharedMaterial = Faction.DefaultSpriteMateral;
        }

        // initialize order renderer
        {
            order_renderer.init();
        }
    }

    public void apply_default_material() => sprite_renderer.sharedMaterial = Faction.DefaultSpriteMateral;
    public void apply_highlighted_material() => sprite_renderer.sharedMaterial = Faction.HighlightedSpriteMaterial;

    OrderRenderer order_renderer;
    SpriteRenderer sprite_renderer;
    bool order_graphic_is_visible;
}
