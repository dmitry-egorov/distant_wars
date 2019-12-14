using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

internal class generate_vision_mesh : MassiveMechanic
{
    public generate_vision_mesh()
    {
        vision_circles_vertices     = new List<Vector3>(0);
        vision_circles_triangles    = new List<int>(0);
        vision_quads_vertices       = new List<Vector3>(0);
        vision_quads_triangles      = new List<int>(0);
        discovery_circles_vertices  = new List<Vector3>(0);
        discovery_circles_triangles = new List<int>(0);
        discovery_quads_vertices    = new List<Vector3>(0);
        discovery_quads_triangles   = new List<int>(0);
    }
    
    public void _()
    {

        var map = Map.Instance;
        if (!map.TexturesReady)
        {
            map.TexturesReady = true; // textures are ready the next frame
            return;
        }

        var time_ratio = Game.Instance.PresentationToSimulationFrameTimeRatio;

        /* units' registry  */ var ur  = UnitsRegistry.Instance;
        /* vision circles mesh    */ var vcm = ur.VisionCirclesMesh;
        /* vision quads mesh      */ var vqm = ur.VisionQuadsMesh;
        /* discovery circles mesh */ var dcm = ur.DiscoveryCirclesMesh;
        /* discovery circles mesh */ var dqm = ur.DiscoveryQuadsMesh;
        /* units            */ var ounits = ur.OwnTeamUnits;
        /* own unit's count */ var oucount = ounits.Count;

        /* local player        */ var lp = LocalPlayer.Instance;
        /* local player's team */ var lpteam_mask = lp.Faction.Team.Mask;

        /* units grid             */ var grid = ur.SpaceGrid;
        /* grid cell centers      */ var ccenters = grid.cell_centers;
        /* fully visible cells    */ var cfviss   = grid.cell_full_visibilities;
        /* fully discovered cells */ var cfdscs   = grid.cell_full_discoveries;
        /* gird cell count        */ var ccount   = cfviss.Length;
        /* cell's radius          */ var cell_radius = grid.cell_radius;

        /* strategic camera */ var sc = StrategicCamera.Instance;
        /* screen rect in world space */ var ws = sc.WorldScreen;

        var vcvrts = vision_circles_vertices;
        var vctris = vision_circles_triangles;
        var vqvrts = vision_quads_vertices;
        var vqtris = vision_quads_triangles;
        var dcvrts = discovery_circles_vertices;
        var dctris = discovery_circles_triangles;
        var dqvrts = discovery_quads_vertices;
        var dqtris = discovery_quads_triangles;

        /* visions count */ var vcount = oucount;
        vcvrts.Clear();
        vctris.Clear();
        vqvrts.Clear();
        vqtris.Clear();
        dcvrts.Clear();
        dctris.Clear();
        dqvrts.Clear();
        dqtris.Clear();
        vcvrts.ReserveMemoryFor(vcount * 4);
        vctris.ReserveMemoryFor(vcount * 6);
        vqvrts.ReserveMemoryFor(vcount * 4);
        vqtris.ReserveMemoryFor(vcount * 6);
        dcvrts.ReserveMemoryFor(vcount * 4);
        dctris.ReserveMemoryFor(vcount * 6);
        dqvrts.ReserveMemoryFor(vcount * 4);
        dqtris.ReserveMemoryFor(vcount * 6);


        /* rect for intersecting quads */ var qscreen = ws.wider_by(cell_radius);
        var vquad_i = 0;
        var dquad_i = 0;
        for (var cell_i = 1; cell_i < ccount; cell_i++)
        {
            // cell is not fully visible
            if ((cfviss[cell_i] & lpteam_mask) == 0)
                continue;
            
            /* cell center */ var c = ccenters[cell_i];

            var visible = qscreen.contains(c);
            var discovered = cfdscs[cell_i];
            if (!discovered)
            {
                cfdscs[cell_i] = true;
            }

            for (var i = 0; i < 4; i++)
            {
                var vert = c.xy(i);
                if (visible)
                {
                    vqvrts.Add(vert);
                }
                if (!discovered)
                {
                    dqvrts.Add(vert);
                }
            }
            
            if (visible)
            {
                RenderHelper.add_quad(vqtris, vquad_i);
                vquad_i++;
            }

            if (!discovered)
            {
                RenderHelper.add_quad(dqtris, dquad_i);
                dquad_i++;
            }
        }

        /* vision    circle index */ var vcircle_i = 0;
        /* discovery circle index */ var dcircle_i = 0;
        for (var i = 0; i < vcount; i++)
        {
            /* unit          */ var u  = ounits[i];
            /* vision radius */ var vs = u.VisionRange;
            var upos  = u.Position;
            var upp   = u.PrevPosition;
            var uipos = Vector2.Lerp(upp, upos, time_ratio);
            
            var fully_visible = true;
            /* own vision range ^2     */ var uvis2 = vs.sqr();
            /* vision + cell radius ^2 */ var uvis_p_crad2 = (vs + cell_radius).sqr();
            /* vision - cell radius    */ var uvis_m_crad  = vs - cell_radius;
            /* vision - cell radius ^2 */ var uvis_m_crad2 = uvis_m_crad >= 0 ? uvis_m_crad.sqr() : -1;
            var it = grid.get_iterator_of_circle(uipos, vs);
            while (it.next(out var cell_i))
            {
                if (cell_i != 0) // is the external cell
                {
                    // cell is fully visible
                    if ((cfviss[cell_i] & lpteam_mask) > 0)
                        continue;

                    /* cell's center */ var ccenter = ccenters[cell_i];
                    /* delta from unit to cell's center       */ var ucdelta = ccenter - upos;
                    /* distance ^2 from unit to cell's center */ var ucdist2 = ucdelta.sqrMagnitude;

                    // cell is outside the vision circle
                    if (ucdist2 > uvis_p_crad2)
                        continue;
                }
                
                fully_visible = false;
                break;
            }

            if (fully_visible)
                continue;

            var qmin = new Vector2(uipos.x - vs, uipos.y - vs);
            var qmax = new Vector2(uipos.x + vs, uipos.y + vs);

            var tl = new Vector3(uipos.x - vs, uipos.y + vs, 0);
            var br = new Vector3(uipos.x + vs, uipos.y - vs, 3);
            var tr = qmax.xy(1);
            var bl = qmin.xy(2);

            // is within the screen rect
            if (ws.intersects(qmin, qmax))
            {
                vcvrts.Add(tl);
                vcvrts.Add(tr);
                vcvrts.Add(bl);
                vcvrts.Add(br);

                RenderHelper.add_quad(vctris, vcircle_i);
                vcircle_i++;
            }

            dcvrts.Add(tl);
            dcvrts.Add(tr);
            dcvrts.Add(bl);
            dcvrts.Add(br);

            RenderHelper.add_quad(dctris, dcircle_i);
            dcircle_i++;
        }

        vcm.Clear();
        vcm.SetVertices(vcvrts);
        vcm.SetTriangles(vctris, 0, false);

        vqm.Clear();
        vqm.SetVertices(vqvrts);
        vqm.SetTriangles(vqtris, 0, false);

        dcm.Clear();
        dcm.SetVertices(dcvrts);
        dcm.SetTriangles(dctris, 0, false);

        dqm.Clear();
        dqm.SetVertices(dqvrts);
        dqm.SetTriangles(dqtris, 0, false);

        /* grid cell size */ var gcsize = 0.5f * grid.cell_size;
        Shader.SetGlobalVector(grid_cell_size_id, new Vector4(gcsize.x, gcsize.y, 0, 0));

        DebugText.set_text
        ("vision",
            $"\n\tv circles: {vcircle_i}"
           +$"\n\tv quads: {vquad_i}"
           +$"\n\td circles: {dcircle_i}"
           +$"\n\td quads: {dquad_i}"
        );
    }

    static readonly int grid_cell_size_id = Shader.PropertyToID("_GridCellSize");

    readonly List<Vector3> vision_circles_vertices;
    readonly List<int>     vision_circles_triangles;
    readonly List<Vector3> vision_quads_vertices;
    readonly List<int>     vision_quads_triangles;
    readonly List<Vector3> discovery_circles_vertices;
    readonly List<int>     discovery_circles_triangles;
    readonly List<Vector3> discovery_quads_vertices;
    readonly List<int>     discovery_quads_triangles;

    bool render_frame;
}