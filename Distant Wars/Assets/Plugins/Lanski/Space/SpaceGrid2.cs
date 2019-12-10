using System.Collections.Generic;
using UnityEngine;

namespace Plugins.Lanski.Space
{
    public class SpaceGrid2<T>
    {
        public Vector2Int Size => size;
        public SpaceGrid2(/* space */ FRect spc, /* size */ Vector2Int sz)
        {
            size = sz;

            /* transform multiplier */ var mul = sz / (spc.max - spc.min);
            /* transform offset     */ var off = -spc.min * mul;
            cell_transform = new SpaceTransform2(mul, off);

            cells = new Cell[sz.x * sz.y + 1];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = new Cell
                {
                    positions = new LeakyList<Vector2>(), 
                    elements = new List<T>() 
                };
            }
        }

        public void clear()
        {
            foreach(var c in cells)
            {
                c.elements.Clear();
                c.positions.Clear();
            }
        }

        public Cell get_cell_of(/* position */ Vector2 p) => cells[get_index_of(p)];

        public void add(/* position */ Vector2  p, /* element */ T e)
        {
            var cell = get_cell_of(p);
            cell.positions.Add(p);
            cell.elements.Add(e);
        }

        private int get_index_of(/* position */ Vector2 p)
        {
            /* cell transform  */ var ct = cell_transform;
            /* cell coordinate */ var cc = Vector2Int.RoundToInt(ct.apply_to_point(p));
            return (cc.x >= 0 && cc.x < size.x && cc.y >= 0 && cc.y < size.y) 
                ? size.x * cc.y + cc.x + 1
                : 0;
        }

        public struct Cell
        {
            public LeakyList<Vector2> positions;
            public List<T> elements;

            public void Deconstruct(out LeakyList<Vector2> positions, out List<T> elements)
            {
                positions = this.positions;
                elements = this.elements;
            }
        }

        Cell[] cells;

        SpaceTransform2 cell_transform;
        Vector2Int size;
    }
}