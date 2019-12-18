using System.Collections.Generic;
using UnityEngine;

public class generate_vision_quads : MassiveMechanic
{
    public generate_vision_quads()
    {
        discovery_quads_triangles = new List<int>(0);
        vision_quads_triangles    = new List<int>(0);
    }
    
    public void _()
    {
        var map = Map.Instance;
        if (!map.TexturesReady)
        {
            map.TexturesReady = true; // textures are ready the next frame
            return;
        }

        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* vision quads mesh      */ var vqm = ur.VisionQuadsMesh;
        /* discovery circles mesh */ var dqm = ur.DiscoveryQuadsMesh;

        /* local player        */ var lp = LocalPlayer.Instance;
        /* local player's team */ var lpteam_mask = lp.Faction.Team.Mask;

        /* units grid          */ var grid = ur.SpaceGrid;
        /* grid cell centers   */ var ccenters = grid.cell_centers;
        /* fully visible cells */ var cfviss   = grid.cell_full_visibilities;
        /* fully visible cells */ var cfrviss  = grid.cell_full_detections;
        /* gird cell count     */ var ccount   = cfviss.Length;
        /* cell's radius       */ var cell_radius = grid.cell_radius;

        /* fully discovered cells */ var cdiscs_lp = grid.cell_full_discoveries_by_local_player;
        
        /* strategic camera */ var sc = StrategicCamera.Instance;
        /* screen rect in world space */ var ws = sc.WorldScreen;

        /* vision    quad index */ var vquad_i = 0;
        /* discovery quad index */ var dquad_i = 0;

        // generate quads
        {
            var vqtris = vision_quads_triangles;
            var dqtris = discovery_quads_triangles;

            vqtris.Clear();
            dqtris.Clear();
            
            /* rect for intersecting quads */ var qscreen = ws.wider_by(cell_radius);
            for (var cell_i = 1; cell_i < ccount; cell_i++)
            {
                if ((cfviss[cell_i] & lpteam_mask) == 0)
                // cell is not fully visible
                    continue;
                
                /* cell's center */ var c = ccenters[cell_i];

                var within_screen = qscreen.contains(c);
                if (within_screen)
                // cell is visible on the screen
                {
                    RenderHelper.add_quad(vqtris, cell_i - 1);
                    vquad_i++;
                }

                if ((cfrviss[cell_i] & lpteam_mask) == 0)
                // cell is not fully visible on radar
                    continue; 

                var discovered = cdiscs_lp[cell_i];
                if (!discovered)
                // cell was not fully discovered before
                {
                    RenderHelper.add_quad(dqtris, cell_i - 1);
                    dquad_i++;

                    cdiscs_lp[cell_i] = true;
                }
            }

            vqm.SetTriangles(vqtris, 0, false);
            dqm.SetTriangles(dqtris, 0, false);
        }

        /* grid cell size */ var gcsize = grid.cell_size;
        Shader.SetGlobalVector(grid_cell_size_id, new Vector4(gcsize.x, gcsize.y, 0, 0));

        var game = Game.Instance;
        game.VisionQuadsCount = vquad_i;
        game.DiscoveryQuadsCount = dquad_i;
    }

    static readonly int grid_cell_size_id = Shader.PropertyToID("_GridCellSize");

    readonly List<int>     vision_quads_triangles;
    readonly List<int>     discovery_quads_triangles;
}
