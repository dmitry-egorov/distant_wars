using System;
using Plugins.Lanski;
using UnityEngine;
using UnityEngine.Assertions;
using static Massive;

[ExecuteInEditMode]
public class Game: RequiredSingleton<Game>
{
    void Update()
    {
        if (!initialized)
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
                Assert.raiseExceptions = true;
                MassiveDebug.Action = s => DebugText.set_text("error", s);
            #endif

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
        
        // debug
        {
            _<pause_on_p>();
            _<disable_debug_text>();
        }

        _<init_new_units>();
        _<set_position_from_transform_in_editor>();

        _<prepare_mouse_clicking>();
        _<prepare_screen_mouse_position>();
        _<prepare_world_mouse_position>();
        _<handle_camera_movement>();
        _<prepare_camera_transformations>();
        _<prepare_world_mouse_position>();
        _<prepare_units_scale>();
        
        _<prepare_mouse_dragging>();
        _<set_cursor_is_a_box>();

        _<find_units_under_the_cursor>();

        _<select_units>();
        _<issue_unit_orders>();

        #if UNITY_EDITOR
            _<select_units_in_inspector>();
        #endif

        // simulation
        if (Application.isPlaying)
        {
            _<cleanup_unit_orders>();
            _<handle_unit_movement>();
            _<update_units_space_grid>();
            _<handle_unit_attacking>();

            _<update_bullets>();

            _<destroy_dead_units>();
            _<cleanup_units>();
        }

        _<update_visible_other_units>();
        _<update_units_style>();

        _<render_order_point>();
        _<render_units>();
        _<render_vision>();
        _<render_bullets>();
        _<render_selection_box>();

        // debug
        {
            _<debug_show_mouse_text>();
        }

        _<resize_discovery_texture>();
        _<resize_vision_texture>();
        
        time += Time.deltaTime;
        DebugText.set_text("time", time.ToString());
    }

    [NonSerialized]bool initialized;

    static float time;
}