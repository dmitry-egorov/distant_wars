using Plugins.Lanski;
using UnityEngine;
using static Massive;

[ExecuteInEditMode]
public class Game: RequiredSingleton<Game>
{
    void Update()
    {
        _<disable_debug_text>();
        _<cleanup_units>();
        _<reset_selection_changed>();
        _<clear_units_in_the_selection_box>();

        _<prepare_mouse_clicking>();
        
        _<prepare_screen_mouse_position>();
        _<prepare_camera_transformations>();
        _<prepare_world_mouse_position>();
        _<handle_camera_movement>(); // Note: camera has moved, so the transformation and mouse position needs to be recalculated
        _<prepare_camera_transformations>();
        _<prepare_world_mouse_position>();
        
        _<prepare_mouse_dragging>();
        _<prepare_units_scale>();

        _<find_units_under_the_cursor>();
        _<find_units_in_the_cursor_box>();
        
        _<handle_unit_selection>();
        _<highlight_units>();
        _<handle_right_click>();

        // simulation
        {
            _<execute_unit_orders>();
        }
        
        _<apply_selection_box_transform>();
        _<apply_unit_transforms>();
        _<show_unit_orders>();
    }
}