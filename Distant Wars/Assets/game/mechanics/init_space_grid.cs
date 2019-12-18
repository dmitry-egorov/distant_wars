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
        var grid = ur.SpaceGrid;

        if (grid == null || grid.size.x != w || grid.size.y != h)
        {
            /* half scale */ var hs   = map.Scale * 0.5f;
            /* map area   */ var spc = new FRect(-hs.xy(), hs.xy());
            var sz = new Vector2Int(w, h);
            grid = ur.SpaceGrid = new UnitsSpaceGrid2();

            grid.size = sz;
            grid.cell_size = (spc.max - spc.min) / sz;
            grid.cell_radius = 0.5f * grid.cell_size.magnitude;
            
            var g2w = grid.grid_to_world = new SpaceTransform2(grid.cell_size, spc.min);
            grid.world_to_grid = g2w.inverse();

            var gccount = sz.x * sz.y + 1;
            var guposs  = grid.unit_positions      = new LeakyList<Vector2>[gccount];
            var gupposs = grid.unit_prev_positions = new LeakyList<Vector2>[gccount];
            var gudets  = grid.unit_detections     = new LeakyList<byte>[gccount];
            var gudiss  = grid.unit_discoveries    = new LeakyList<byte>[gccount];
            var guteams = grid.unit_team_masks     = new LeakyList<byte>[gccount];
            var gunits  = grid.unit_refs           = new List<Unit>[gccount];

            var cc = grid.cell_centers = new Vector2[gccount];
            grid.cell_full_visibilities = new byte[gccount];
            grid.cell_full_detections = new byte[gccount];
            grid.cell_full_discoveries_by_local_player = new bool[gccount];
            var vqvrts = grid.vision_quads_vertices = new Vector3[4 * (gccount - 1)];

            // intialize unit data arrays
            for (var cell_i = 0; cell_i < gccount; cell_i++)
            {
                if (cell_i > 0) // set cell center; the external cell is at (0,0), so we dont need to initialize it
                {
                    /* cell's grid  center */ var ic = new Vector2((cell_i - 1) % sz.y + 0.5f, (cell_i - 1) / sz.y + 0.5f);
                    /* cell's world center */ var wc = g2w.apply_to_point(ic);
                    cc[cell_i] = wc;

                    for (var i = 0; i < 4; i++)
                        vqvrts[4 * (cell_i - 1) + i] = wc.xy(i);
                }

                guposs [cell_i] = new LeakyList<Vector2>();
                gupposs[cell_i] = new LeakyList<Vector2>();
                gudets [cell_i] = new LeakyList<byte>();
                gudiss [cell_i] = new LeakyList<byte>();
                guteams[cell_i] = new LeakyList<byte>();
                gunits [cell_i] = new List<Unit>();
            }

            ur.VisionQuadsMesh   .vertices = vqvrts;
            ur.DiscoveryQuadsMesh.vertices = vqvrts;

            // fill unit data arrays
            foreach(var unit in ur.Units)
            {
                /* unit's position */ var pos = unit.Position;
                var uteam = unit.Faction.Team.Mask;
                var ui = grid.get_index_of(pos);

                var cunits = gunits[ui];
                unit.SpaceGridIndex = (ui, cunits.Count);
                cunits.Add(unit);
                guposs [ui].Add(pos);
                gupposs[ui].Add(pos);
                guteams[ui].Add(uteam);
                gudets [ui].Add(uteam);
                gudiss [ui].Add(uteam);
            }
        }
    }
}