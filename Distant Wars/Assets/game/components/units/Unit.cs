using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class Unit : MassiveBehaviour<UnitsRegistry, Unit>
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

    public void show_order_graphic()
    {
        if (MoveTarget.TryGetValue(out var t))
        {
            order_renderer.show_move_order(Position, t);
        }
    }

    public void hide_order_graphic()
    {
        order_renderer.hide();
    }

    void Start()
    {
        // find components
        {
            Assert.IsNotNull(sprite_renderer = GetComponent<SpriteRenderer>());
            Assert.IsNotNull(order_renderer = GetComponentInChildren<OrderRenderer>());
        }

        // apply faction material
        {
            sprite_renderer.sharedMaterial = Faction != null ? Faction.DefaultSpriteMaterial : null;
        }

        // initialize order renderer
        {
            order_renderer.init();
        }
        
        if (!Application.isPlaying) return;
        
        // assert data is set
        {
            Assert.IsNotNull(Faction);
        }
        
        // apply transform to position
        {
            Position = transform.position.xy();
        }
    }

    public void apply_default_material() => sprite_renderer.sharedMaterial = Faction.DefaultSpriteMaterial;
    public void apply_highlighted_material() => sprite_renderer.sharedMaterial = Faction.HighlightedSpriteMaterial;
    public void apply_selected_material() => sprite_renderer.sharedMaterial = Faction.SelectedSpriteMaterial;

    OrderRenderer order_renderer;
    SpriteRenderer sprite_renderer;
}
