using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Lanski.Space
{
    public class SpaceGrid2<T>
    {
        public SpaceTransform2 world_to_grid;
        public Vector2Int size;
        public Vector2 cell_size;
        public float cell_radius;
        public LeakyList<Vector2>[] cells_positions;
        public List<T>[] cells_elements;
        public Vector2[] cells_centers;

        public SpaceGrid2(/* space */ Rect spc, /* size */ Vector2Int sz)
        {
            size = sz;
            cell_size = (spc.max - spc.min) / sz;
            cell_radius = cell_size.magnitude;
            
            var g2w = new SpaceTransform2(cell_size, spc.min);
            world_to_grid = g2w.inverse();

            var count = sz.x * sz.y + 1;
            cells_positions = new LeakyList<Vector2>[count];
            cells_elements  = new List<T>[count];
            cells_centers   = new Vector2[count];

            for (int i = 0; i < count; i++)
            {
                /* cell's grid  center */ var ic = new Vector2((i - 1) % sz.y + 0.5f, (i - 1) / sz.y + 0.5f);
                /* cell's world center */ var wc = g2w.apply_to_point(ic);

                cells_positions[i] = new LeakyList<Vector2>();
                cells_elements [i] = new List<T>();
                cells_centers  [i] = wc;
            }
        }

        public void clear()
        {
            for (var i = 0; i < cells_positions.Length; i++)
            {
                cells_positions[i].Clear();
                cells_elements [i].Clear();
            }
        }

        public void add(/* position */ Vector2  p, /* element */ T e)
        {
            var i = get_index_of(p);
            cells_positions[i].Add(p);
            cells_elements [i].Add(e);
        }

        public int get_index_of(/* position */ Vector2 p) => get_index_of(get_coord_of(p));

        public int get_index_of(/* position */ Vector2Int cc) => get_index_of(cc.x, cc.y);

        public int get_index_of(int cx, int cy) =>
            (cx >= 0 && cx < size.x && cy >= 0 && cy < size.y)
                ? size.x * cy + cx + 1
                : 0
        ;

        public RectInt get_rect_of(Rect r) => new RectInt(get_coord_of(r.min), get_coord_of(r.max));
        public Vector2Int get_coord_of(Vector2 p)
        {
            /* cell transform  */ var ct = world_to_grid;
            /* cell coordinate */ return Vector2Int.RoundToInt(ct.apply_to_point(p));
        }
    }
}