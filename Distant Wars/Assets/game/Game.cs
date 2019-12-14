using UnityEngine;
using UnityEngine.Assertions;

[ExecuteInEditMode]
public class Game: MassiveGame<Game>
{
    [Header("State")]
    public int VisionCirclesCount;
    public int DiscoveryCirclesCount;
    public int VisionQuadsCount;
    public int DiscoveryQuadsCount;

    void Update()
    {
        game_loop(
            init: () => 
            {
                #if DEVELOPMENT_BUILD || UNITY_EDITOR
                    Assert.raiseExceptions = true;
                    MassiveDebug.Action = s => DebugText.set_text("error", s);
                #endif

                _<init_map>();
                _<init_local_player>();
                _<init_units_registry>();
                _<init_bullets_manager>();
                _<init_selection_box>();
                _<init_order_point>();
                _<init_camera>();

                //Note: camera movement requires transformations from the previous frame, that's why the call is duplicated in the initialisation
                _<prepare_camera_transformations>();
            },
            update_input: () => 
            {
                _<init_space_grid>();

                #if UNITY_EDITOR
                if (Application.isPlaying)
                #endif
                {
                    // debug
                    {
                        _<pause_on_key>();
                        _<disable_debug_text>();
                        _<change_grid_size_on_key>();
                        _<hide_vision_on_key>();
                    }

                    _<prepare_mouse_clicking>();
                    _<prepare_screen_mouse_position>();
                    _<prepare_world_mouse_position>();
                    
                    _<prepare_mouse_dragging>();
                    _<set_cursor_is_a_box>();

                    _<find_units_under_the_cursor>();
                    _<select_units>();
                    _<issue_unit_orders>();

                    _<handle_camera_movement>();
                    _<prepare_camera_transformations>();

                    #if UNITY_EDITOR
                        _<select_units_in_inspector>();
                    #endif
                }
            },
            update_simulation: () =>
            {
                #if UNITY_EDITOR
                if (Application.isPlaying)
                #endif
                {
                    _<init_new_units>();

                    _<handle_movement>();
                    _<update_space_grid>();
                    
                    _<update_projectiles>();
                    _<handle_incoming_damage>();

                    _<cleanup_units>();
                    _<cleanup_unit_orders>();

                    _<update_unit_visibility>();
                    _<find_and_attack_target>();
                }
            },
            present: (had_steady_update) => 
            {
                _<update_order_point>();
                
                _<init_discovery_texture>();
                _<init_vision_texture>();

                _<update_units_style>();

                _<generate_vision_quads>();
                _<generate_vision_circles>();

                _<update_unit_blinking>();
                _<generate_units_mesh>();
                
                _<generate_projectiles_mesh>();
                _<update_selection_box>();
            }
        );

        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            _<set_position_from_transform_in_editor>();
            _<init_new_units>();
            _<update_space_grid>();
        }
        #endif

        // debug
        {
            _<show_debug_text>();
        }
    }
}