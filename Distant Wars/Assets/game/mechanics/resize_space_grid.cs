using System.Collections.Generic;
using Plugins.Lanski;
using Plugins.Lanski.Space;
using UnityEngine;

internal class init_space_grid : MassiveMechanic
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
            
            var g2w = sg.grid_to_world = new SpaceTransform2(sg.cell_size, spc.min);
            sg.world_to_grid = g2w.inverse();


            var gucount = sz.x * sz.y + 1;
            var guposs  = sg.unit_positions      = new LeakyList<Vector2>[gucount];
            var gupposs = sg.unit_prev_positions = new LeakyList<Vector2>[gucount];
            var guviss  = sg.unit_visibilities   = new LeakyList<byte>[gucount];
            var guteams = sg.unit_team_masks     = new LeakyList<byte>[gucount];
            var gunits  = sg.unit_refs           = new List<Unit>[gucount];

            sg.cell_centers = new Vector2[gucount];
            sg.cell_full_visibilities = new byte[gucount];
            sg.cell_full_discoveries  = new bool[gucount];

            for (int i = 0; i < gucount; i++)
            {
                if (i > 0)
                {
                    /* cell's grid  center */ var ic = new Vector2((i - 1) % sz.y + 0.5f, (i - 1) / sz.y + 0.5f);
                    /* cell's world center */ var wc = g2w.apply_to_point(ic);
                    sg.cell_centers[i] = wc;
                }

                guposs [i] = new LeakyList<Vector2>();
                gupposs[i] = new LeakyList<Vector2>();
                guviss [i] = new LeakyList<byte>();
                guteams[i] = new LeakyList<byte>();
                gunits [i] = new List<Unit>();
            }

            foreach(var unit in ur.Units)
            {
                /* unit's position */ var pos = unit.Position;
                var uteam = unit.Faction.Team.Mask;
                var ui = sg.get_index_of(pos);

                var cunits = gunits[ui];
                unit.SpaceGridIndex = (ui, cunits.Count);
                cunits.Add(unit);
                guposs [ui].Add(pos);
                gupposs[ui].Add(pos);
                guteams[ui].Add(uteam);
                guviss [ui].Add(uteam);
            }

            Debug.Log("Space grid resized");
        }
    }
}