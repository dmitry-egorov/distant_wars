using UnityEngine;
using UnityEngine.Assertions;
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

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
              init
            , handle_input
            , simulate
            , render
        );
        
        void init()
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            {
                Assert.raiseExceptions = true;
                MassiveDebug.Action = s => DebugText.set_text("error", s);
            }
            #endif

            run<init_map>();
            run<init_local_player>();
            run<init_units_registry>();
            run<init_projectiles_manager>();
            run<init_explosions_manager>();
            run<init_selection_box>();
            run<init_order_point>();
            run<init_camera>();
            run<init_faction_colors>();

            // Note: camera movement requires transformations from the previous frame, that's why the call is duplicated in the init
            run<prepare_camera_transformations>();
        }

        void handle_input()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            #endif
            {
                run<init_space_grid>();

                // debug
                {
                    run<pause_on_key>();
                    run<disable_debug_text>();
                    run<change_grid_size_on_key>();
                    run<hide_vision_on_key>();
                    run<show_grids_on_key>();
                    run<toggle_target_prediction>();
                }

                run<prepare_mouse_clicking>();
                run<prepare_screen_mouse_position>();
                run<prepare_world_mouse_position>();

                run<prepare_mouse_dragging>();
                run<set_cursor_is_a_box>();

                run<find_units_under_the_cursor>();
                run<select_units>();
                run<issue_unit_orders>();

                run<handle_camera_movement>();
                run<prepare_camera_transformations>();
            }
        }

        void simulate()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            #endif
            {
                run<init_new_units>();

                run<handle_movement>();
                run<update_space_grid>();

                run<update_explosions>();
                run<update_projectiles>();
                run<handle_incoming_damage>();

                run<cleanup_dead_units>();
                run<cleanup_unit_orders>();

                run<update_unit_visibility>();
                run<find_and_attack_target>();
            }
        }

        void render(bool had_steady_update)
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            #endif
            {
                run<update_order_point>();

                run<init_discovery_texture>();
                run<init_vision_texture>();

                run<update_units_style>();

                run<generate_vision_quads>();
                run<generate_vision_circles>();

                run<update_unit_blinking>();
                run<prepare_adujsted_sprite_size>();
                run<generate_units_mesh>();

                run<generate_explosions_mesh>();
                run<generate_projectiles_mesh>();
                run<update_selection_box>();

                // debug
                {
                    run<show_debug_text>();
                }
            }

            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                run<disable_debug_text>();
                run<editor_setup_map_rendering>();

                run<editor_set_position_from_transform>();
                run<init_new_units>();
                run<prepare_adujsted_sprite_size>();
                run<editor_generate_units_mesh>();
            }
            else
            {
                run<select_units_in_inspector>();
            }
            #endif
        }
    }
}