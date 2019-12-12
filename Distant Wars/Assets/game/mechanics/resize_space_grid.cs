using System.Collections.Generic;
using Plugins.Lanski;
using Plugins.Lanski.Space;
using UnityEngine;

internal class resize_space_grid : MassiveMechanic
{
    public void _()
    {
        /* units' registry */ var ur  = UnitsRegistry.Instance;
        /* map             */ var map = Map.Instance;

        var h  = ur.SpaceGridHeight;
        var w  = ur.SpaceGridWidth;
        var sg = ur.SpaceGrid;

        if (sg == null || sg.size.x != w || sg.size.y != h)
        {
            /* half scale */ var hs   = map.Scale * 0.5f;
            /* map area   */ var spc = new FRect(-hs.xy(), hs.xy());
            var sz = new Vector2Int(w, h);
            sg = ur.SpaceGrid = new UnitsSpaceGrid2();

            sg.size = sz;
            sg.cell_size = (spc.max - spc.min) / sz;
            sg.cell_radius = 0.5f * sg.cell_size.magnitude;
            
            var g2w = new SpaceTransform2(sg.cell_size, spc.min);
            sg.world_to_grid = g2w.inverse();

            var count = sz.x * sz.y + 1;
            var ps = sg.unit_positions    = new LeakyList<Vector2>[count];
            var vs = sg.unit_visibilities = new LeakyList<byte>[count];
            var ts = sg.unit_team_masks   = new LeakyList<byte>[count];
            var us = sg.unit_refs         = new List<Unit>[count];

            sg.cell_centers = new Vector2[count];
            sg.cell_full_visibilities = new byte[count];

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    /* cell's grid  center */ var ic = new Vector2((i - 1) % sz.y + 0.5f, (i - 1) / sz.y + 0.5f);
                    /* cell's world center */ var wc = g2w.apply_to_point(ic);
                    sg.cell_centers[i] = wc;
                }

                ps[i] = new LeakyList<Vector2>();
                vs[i] = new LeakyList<byte>();
                ts[i] = new LeakyList<byte>();
                us[i] = new List<Unit>();
            }

            foreach(var u in ur.Units)
            {
                /* unit's position */ var p = u.Position;
                var ut = u.Faction.Team.Mask;
                var ui = sg.get_index_of(p);
                ps[ui].Add(p);
                ts[ui].Add(ut);
                us[ui].Add(u);
                vs[ui].Add(ut);
            }
        }
    }
}