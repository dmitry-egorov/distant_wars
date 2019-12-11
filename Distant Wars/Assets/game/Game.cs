using System;
using System.Linq;
using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Assertions;
using static Massive;

[ExecuteInEditMode]
public class Game: RequiredSingleton<Game>
{
    public double MeasuringWeight = 0.1;

    void Update()
    {
        if (!initialized)
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
                Assert.raiseExceptions = true;
                MassiveDebug.Action = s => DebugText.set_text("error", s);
            #endif

            moving_average_weight = MeasuringWeight;

            _<initialize_map>();
            _<initialize_local_player>();
            _<initialize_units_registry>();
            _<initialize_bullets_manager>();
            _<initialize_selection_box>();
            _<initialize_order_point>();
            _<initialize_camera>();

            //Note: camera movement requires transformations from the previous frame, that's why the call is duplicated in the initialisation
            _<prepare_camera_transformations>();
            
            _<cleanup_units>();
            
            initialized = true;
        }

        start_update();
        
        // debug
        {
            _<pause_on_p>();
            _<disable_debug_text>();
        }

        _<resize_space_grid>();

        _<set_position_from_transform_in_editor>();

        _<prepare_mouse_clicking>();
        _<prepare_screen_mouse_position>();
        _<prepare_world_mouse_position>();
        _<handle_camera_movement>();
        _<prepare_camera_transformations>();
        _<prepare_world_mouse_position>();
        
        _<prepare_mouse_dragging>();
        _<set_cursor_is_a_box>();

        _<find_units_under_the_cursor>();

        _<select_units>();
        _<issue_unit_orders>();

        #if UNITY_EDITOR
            _<select_units_in_inspector>();
        #endif

        // simulation
        {
            _<init_new_units>();

            if (Application.isPlaying)
            {
                _<cleanup_unit_orders>();
                _<handle_movement>();
                _<update_space_grid>();
                _<find_visible_units_from_other_teams>();
                _<find_and_attack_target>();

                _<update_projectiles>();
                _<handle_incoming_damage>();

                _<destroy_dead_units>();
                _<cleanup_units>();
            }
        }
        
        #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _<update_space_grid>();
            }
        #endif

        //_<find_visible_units_from_other_teams>();
        _<update_units_style>();

        _<render_order_point>();
        _<generate_units_mesh>();
        _<generate_vision_mesh>();
        _<generate_projectiles_mesh>();
        _<render_selection_box>();

        // debug
        {
            _<debug_show_mouse_text>();
        }

        _<resize_discovery_texture>();
        _<resize_vision_texture>();

        finish_update();

        /*debug_time<find_other_teams_visible_units>();
        debug_time<update_space_grid>();
        if (Application.isPlaying)
        {
            debug_time<find_and_attack_target>();
            debug_time<update_bullets>();
        }*/

        DebugText.set_text("avg frame time", get_avg_frame_time());

        DebugText.set_text("avg update time", get_avg_update_time());

        var top_avg_times = string.Join("", get_top_avg_times(10).Select(x => $"\n\t{x.name}: {x.time}"));
        DebugText.set_text("top times", top_avg_times);

        DebugText.set_text("total units", UnitsRegistry.Instance.Units.Count.ToString());
        DebugText.set_text("total projectiles", ProjectilesManager.Instance.Positions.Count.ToString());
    }

    void debug_time<T>() where T: class, MassiveMechanic, new()
    {
        var (n,t) = get_avg_time_for<T>();
        DebugText.set_text("time of " + n, t);
    }

    [NonSerialized]bool initialized;

    static float time;
}