using System.Collections.Generic;
using Plugins.Lanski;
using Plugins.Lanski.Space;
using UnityEngine;

public class UnitsSpaceGrid2
{
    public SpaceTransform2 world_to_grid;
    public SpaceTransform2 grid_to_world;
    public Vector2Int size;
    public Vector2 cell_size;
    public float cell_radius;

    // Units
    public LeakyList<Vector2>[] unit_positions;
    public LeakyList<Vector2>[] unit_prev_positions;
    public LeakyList<byte>[] unit_detections_by_team; // whether the unit is detected by radar or vision
    public LeakyList<byte>[] unit_identifications_by_team; // whether the unit is discovered, i.e. was in the vision range once
    public LeakyList<byte>[] unit_teams;
    public List<Unit>[] unit_refs;

    // Cell dynamic
    public byte[] cell_full_visibilities_by_team;
    public byte[] cell_full_detections_by_team;
    public bool[] cell_full_discoveries_by_local_player;

    // Cell static
    public Vector2[] cell_centers;

    public int get_index_of(/* position */ Vector2 p) => get_index_of(get_coord_of(p));

    public int get_index_of(/* position */ Vector2Int cc) => get_index_of(cc.x, cc.y);

    public int get_index_of(int cx, int cy) =>
        (cx >= 0 && cx < size.x && cy >= 0 && cy < size.y)
            ? size.x * cy + cx + 1
            : 0
    ;

    public Iterator get_iterator_of_circle(/* center */ Vector2 c, /* radius */ float r) => get_itertor_of(get_coord_rect_of_circle(c, r));
    public Iterator get_iterator_of(FRect r) => get_itertor_of(get_coord_rect_of(r));

    public IntRect get_coord_rect_of_circle(/* center */ Vector2 c, /* radius */ float r) 
    {
        var offset = new Vector2(r, r);
        return new IntRect(get_coord_of(c - offset), get_coord_of(c + offset));
    }

    public IntRect get_coord_rect_of(FRect r) => new IntRect(get_coord_of(r.min), get_coord_of(r.max));
    public Vector2Int get_coord_of(Vector2 p)
    {
        /* cell transform  */ var ct = world_to_grid;
        /* cell coordinate */ return Vector2Int.FloorToInt(ct.apply_to_point(p));
    }

    private Iterator get_itertor_of(IntRect rect)
    {
        var minx = rect.min.x;
        var miny = rect.min.y;
        var maxx = rect.max.x;
        var maxy = rect.max.y;

        /* process external cell */ var external = false;

        int width = size.x;
        int height = size.y;

        if (minx >= width || maxx < 0 || miny >= height || maxy < 0) 
        {
            return new Iterator(-1, 0, 0, 0, true);
        }

        if (minx < 0)
        {
            external = true;
            minx = 0;
        }
        if (miny < 0)
        {
            external = true;
            miny = 0;
        }
        if (maxx >= width)
        {
            external = true;
            maxx = width - 1;
        }
        if (maxy >= height)
        {
            external = true;
            maxy = height - 1;
        }

        /* vision area width  */ var va_width  = maxx - minx + 1; // add 1, since maxx is inclusive
        /* vision area height */ var va_height = maxy - miny + 1; // add 1, since maxy is inclusive

        // add 1, since 0 is the external cell
        var ci_offset = minx + miny * width + 1; 
        var end_i = va_width * va_height;

        return new Iterator(end_i, ci_offset, va_width, width, external);
    }

    public struct Iterator
    {
        public Iterator(
            int end_i, 
            int cell_i_offset, 
            int area_width, 
            int grid_width, 
            bool external
        )
        {
            this.i = 0;
            this.end_i = end_i;
            this.cell_i_offset = cell_i_offset;
            this.area_width = area_width;
            this.grid_width = grid_width;
            this.external = external;
        }

        public bool next(out int cell_index)
        {
            if (i < end_i)
            {
                cell_index = cell_i_offset + i % area_width + (i / area_width) * grid_width;
                i++;
                return true;
            }
            else
            {
                if (external)
                {
                    external = false;
                    cell_index = 0;
                    return true;;
                }

                cell_index = -1;
                return false;
            }
        }

        int i;
        int end_i;
        int cell_i_offset;
        int area_width;
        int grid_width;
        bool external;
    }

    public static void remove_unit_from_cell
    (
        int unit_i
        , List<Unit> cell_unit_refs
        , LeakyList<Vector2> cell_unit_poss
        , LeakyList<Vector2> cell_unit_prev_poss
        , LeakyList<byte> cell_unit_teams
        , LeakyList<byte> cell_unit_detects
        , LeakyList<byte> cell_unit_idents
    )
    {
        cell_unit_refs.ReplaceWithLast(unit_i);
        cell_unit_poss.ReplaceWithLast(unit_i);
        cell_unit_prev_poss.ReplaceWithLast(unit_i);
        cell_unit_teams.ReplaceWithLast(unit_i);
        cell_unit_detects.ReplaceWithLast(unit_i);
        cell_unit_idents.ReplaceWithLast(unit_i);

        // fix the id of the replacing unit
        if (unit_i < cell_unit_refs.Count) cell_unit_refs[unit_i].space_grid_index.index = unit_i;
    }
}