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
            Assert.raiseExceptions = true;
            MassiveDebug.Action = s => DebugText.Instance.set_text(s);

            _<initialize_map>();
            _<initialize_local_player>();
            _<initialize_units_registry>();
            _<initialize_selection_box>();
            _<initialize_order_point>();
            
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
        _<prepare_camera_transformations>();
        _<prepare_world_mouse_position>();
        _<handle_camera_movement>(); // Note: camera has moved, so the transformation and mouse position needs to be recalculated
        _<prepare_camera_transformations>();
        _<prepare_world_mouse_position>();
        
        _<prepare_mouse_dragging>();
        _<prepare_units_scale>();

        _<set_cursor_is_a_box>();
        _<find_units_under_the_cursor>();
        _<find_units_in_the_cursor_box>();
        
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
    }

    [NonSerialized]bool initialized;
}