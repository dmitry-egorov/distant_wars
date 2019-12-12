using System.Collections.Generic;
using Plugins.Lanski;
using Plugins.Lanski.Space;
using UnityEngine;

public class UnitsSpaceGrid2
{
    public SpaceTransform2 world_to_grid;
    public Vector2Int size;
    public Vector2 cell_size;
    public float cell_radius;

    // Units
    public LeakyList<Vector2>[] unit_positions;
    public LeakyList<byte>[]    unit_visibilities;
    public LeakyList<byte>[]    unit_team_masks;
    public List<Unit>[]         unit_refs;

    // Cell dynamic
    public byte[]    cell_full_visibilities;

    // Cell static
    public Vector2[] cell_centers;

    public int get_index_of(/* position */ Vector2 p) => get_index_of(get_coord_of(p));

    public int get_index_of(/* position */ Vector2Int cc) => get_index_of(cc.x, cc.y);

    public int get_index_of(int cx, int cy) =>
        (cx >= 0 && cx < size.x && cy >= 0 && cy < size.y)
            ? size.x * cy + cx + 1
            : 0
    ;

    public IntRect get_coord_rect_of_circle(/* center */ Vector2 c, /* radius */ float r) => new IntRect(get_coord_of(c + new Vector2(-r, -r)), get_coord_of(c + new Vector2(r, r)));
    public IntRect get_coord_rect_of(FRect r) => new IntRect(get_coord_of(r.min), get_coord_of(r.max));
    public Vector2Int get_coord_of(Vector2 p)
    {
        /* cell transform  */ var ct = world_to_grid;
        /* cell coordinate */ return Vector2Int.FloorToInt(ct.apply_to_point(p));
    }
}