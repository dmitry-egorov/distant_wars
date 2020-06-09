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
    public int UnitSpritesCount;
    public int HPBarsCount;

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
                _<init_projectiles_manager>();
                _<init_explosions_manager>();
                _<init_selection_box>();
                _<init_order_point>();
                _<init_camera>();
                _<init_faction_colors>();

                //Note: camera movement requires transformations from the previous frame, that's why the call is duplicated in the initialisation
                _<prepare_camera_transformations>();
            },
            update_input: () => 
            {
                #if UNITY_EDITOR
                if (Application.isPlaying)
                #endif
                {
                    _<init_space_grid>();

                    // debug
                    {
                        _<pause_on_key>();
                        _<disable_debug_text>();
                        _<change_grid_size_on_key>();
                        _<hide_vision_on_key>();
                        _<show_grids_on_key>();
                        _<toggle_target_prediction>();
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
                    
                    _<update_explosions>();
                    _<update_projectiles>();
                    _<handle_incoming_damage>();

                    _<cleanup_dead_units>();
                    _<cleanup_unit_orders>();

                    _<update_unit_visibility>();
                    _<find_and_attack_target>();
                }
            },
            present: (had_steady_update) => 
            {
                #if UNITY_EDITOR
                if (Application.isPlaying)
                #endif
                {
                    _<update_order_point>();
                    
                    _<init_discovery_texture>();
                    _<init_vision_texture>();

                    _<update_units_style>();

                    _<generate_vision_quads>();
                    _<generate_vision_circles>();

                    _<update_unit_blinking>();
                    _<prepare_adujsted_sprite_size>();
                    _<generate_units_mesh>();
                    
                    _<generate_explosions_mesh>();
                    _<generate_projectiles_mesh>();
                    _<update_selection_box>();

                    // debug
                    {
                        _<show_debug_text>();
                    }
                }

                #if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        _<disable_debug_text>();
                        _<editor_setup_map_rendering>();

                        _<editor_set_position_from_transform>();
                        _<init_new_units>();
                        _<prepare_adujsted_sprite_size>();
                        _<editor_generate_units_mesh>();
                    }
                    else
                    {
                        _<select_units_in_inspector>();
                    }
                #endif
            }
        );
    }
}