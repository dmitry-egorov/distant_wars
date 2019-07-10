using Plugins.Lanski;
using static Massive;

public class Game: RequiredSingleton<Game>
{
    public bool ManualPhysics;
    
    void Update()
    {
        _<cleanup_units>();
        _<reset_selection_changed>();
        _<clear_units_in_the_selection_box>();

        _<prepare_mouse_position>();
        _<prepare_mouse_clicking>();
        _<prepare_mouse_dragging>();
        _<handle_camera_movement>();
        _<prepare_camera_transformations>();
        
        _<find_units_under_the_cursor>();
        _<find_units_in_the_cursor_box>();
        
        _<handle_unit_selection>();
        _<highlight_units>();
        _<handle_right_click>();

        _<execute_unit_orders>();
        
        _<apply_selection_box_transform>();
        _<prepare_units_scale>();
        _<apply_unit_transforms>();
        _<show_unit_orders>();
    }
}