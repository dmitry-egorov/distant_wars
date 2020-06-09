using System.Collections.Generic;
using Plugins.Lanski;
using UnityEngine;

public class generate_units_mesh: MassiveMechanic
{
    public generate_units_mesh()
    {
        sprite_vertices = new List<Vector3>(0);
        sprite_triangles = new List<int>(0);
        hp_sprite_vertices = new List<Vector3>(0);
        hp_sprite_triangles = new List<int>(0);
    }
    
    public void _()
    {
        var game = Game.Instance;
        var time_ratio = game.PresentationToSimulationFrameTimeRatio;

        /* local player    */ var lp = LocalPlayer.Instance;
        /* local player's team mask */ var lp_team_mask = lp.Faction.Team.Mask;

        /* units' registry */ var ur  = UnitsRegistry.Instance;
        /* camera          */ var cam = StrategicCamera.Instance;
        /* sprite size multiplier */ var ssizem = cam.SpriteSizeMultiplier;
        /* adjasted sprite size   */ var ssize = ur.AdjustedSpriteSize;
        
        var sv = sprite_vertices;
        var st = sprite_triangles;
        var hpv = hp_sprite_vertices;
        var hpt = hp_sprite_triangles;

        sv.Clear();
        st.Clear();
        hpv.Clear();
        hpt.Clear();

        /* units space grid    */ var grid    = ur.SpaceGrid;
        /* grid positions      */ var gposs   = grid.unit_positions;
        /* grid prev positions */ var gpposs  = grid.unit_prev_positions;
        /* grid units          */ var gunits  = grid.unit_refs;
        /* grid visiblities    */ var gdets   = grid.unit_detections_by_team;

        var max_cprop = ur.HPBarMaxCamZoomLevel;
        var cprop = cam.SizeProportion;
        var render_hp = cprop <= max_cprop;

        //TODO!: undiscovered units

        /* screen rectangle */ var rscreen = cam.WorldScreen;
        /* screen rect offset to accomodate unit's size */ var offset = ssize * MathEx.Root2;
        /* adjusted screen rectangle */ var arscreen = rscreen.wider_by(offset);
        /* screen grid area iterator */ var it = grid.get_iterator_of(arscreen);
        /* sprite index */ var isprite = 0;
        /* hp sprite index */ var hsprite = 0;

        while (it.next(out var cell_i))
        {
            /* cell unit positions      */ var cuposs  = gposs [cell_i];
            /* cell unit prev positions */ var cupposs = gpposs[cell_i];
            /* cell units               */ var cunits  = gunits[cell_i];
            /* cell unit visibilities   */ var cudets  = gdets [cell_i];
            /* cell units count         */ var cucount = cuposs.Count;
            for (var iunit = 0; iunit < cucount; iunit++)
            {
                // hide when not detected by the local player
                if ((cudets[iunit] & lp_team_mask) == 0)
                    continue;

                /* the unit */ var unit = cunits[iunit];

                /* unit's position      */ var upos  = cuposs [iunit];
                /* unit's prev position */ var uppos = cupposs[iunit];
                /* unit's interpolated position */ var uipos = Vector2.Lerp(uppos, upos, time_ratio);

                // render hp
                if (render_hp)
                {
                    var hp_prop = unit.hit_points / (float)unit.MaxHitPoints;
                    if (hp_prop < 1)
                    {
                        hp_prop = hp_prop < 0 ? 0 : hp_prop;
                        for (var i = 0; i < 8; i++)
                        {
                            hpv.Add(uipos.xy(i + hp_prop));
                        }

                        hpt.add_quad(hsprite);
                        hsprite++;
                        hpt.add_quad(hsprite);
                        hsprite++;
                    }
                }
                
                if (unit.is_blinking)
                    continue;

                //TODO: display gray icon when not discovered

                /* unit is highlighted */ var uih = unit.is_highlighted;
                /* unit is selected    */ var uis = unit.is_selected;
                /* faction color index */ var fci = unit.Faction.Index;
                /* highlight flags     */ var hf  = (uih ? 1 : 0) | (uis ? 2 : 0) | (fci << 4);
                
                for (var i = 0; i < 4; i++)
                {
                    // flags for the vertex shader, right to left: is highlighted (1 bit), is selected (1 bit), quad index (2 bits), color index (4 bits) 
                    var fl = hf | i << 2;
                    sv.Add(uipos.xy(fl));
                }

                //PERF: since the quads are not changing, we should only generate them when capacity is increasing.
                st.add_quad(isprite);
                isprite++;
            }
        }

        /* sprites mesh */ var smesh  = ur.SpritesMesh;
        smesh.Clear();
        smesh.SetVertices(sv);
        smesh.SetTriangles(st, 0, false);

        var hpmesh = ur.HPSpritesMesh;
        hpmesh.Clear();
        hpmesh.SetVertices(hpv);
        hpmesh.SetTriangles(hpt, 0, false);

        Shader.DisableKeyword("HIDE_MAP_VISION");
        Shader.SetGlobalFloat("_HPBarWidthPx",  ur.HPBarWidthPx  * ssizem);
        Shader.SetGlobalFloat("_HPBarHeightPx", ur.HPBarHeightPx * ssizem);
        Shader.SetGlobalFloat("_HPBarOffsetPx", ur.HPBarOffsetPx * ssizem);

        game.UnitSpritesCount = isprite;
        game.HPBarsCount = hsprite / 2;
    }

    readonly List<Vector3> sprite_vertices;
    readonly List<int> sprite_triangles;

    readonly List<Vector3> hp_sprite_vertices;
    readonly List<int> hp_sprite_triangles;
    
}