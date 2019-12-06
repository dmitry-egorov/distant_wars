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
            _<initialize_selection_box>();
            _<initialize_order_point>();
            
            _<prepare_camera_transformations>();//Note: camera movement requires transformations from the previous frame
            
            initialized = true;
        }
        
        // debug
        {
            _<pause_on_p>();
            _<disable_debug_text>();
        }

        _<cleanup_units>();
        _<start_new_units>();
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
        _<set_units_style>();
        _<issue_move_orders>();

        // simulation
        {
            _<execute_unit_orders>();
        }
        
        _<render_order_point>();
        _<render_units>();
        _<render_selection_box>();

        // debug
        {
            _<debug_show_mouse_text>();
        }
    }

    [NonSerialized]bool initialized;
}