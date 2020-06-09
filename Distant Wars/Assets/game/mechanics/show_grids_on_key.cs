using UnityEngine;

internal class show_grids_on_key : IMassiveMechanic
{
    public void _()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            show_space_grid = !show_space_grid;
            if (show_space_grid)
            {
                Shader.EnableKeyword("SHOW_SPACE_GRID");
            }
            else
            {
                Shader.DisableKeyword("SHOW_SPACE_GRID");
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            show_map_grid = !show_map_grid;
            if (show_map_grid)
            {
                Shader.EnableKeyword("SHOW_MAP_GRID");
            }
            else
            {
                Shader.DisableKeyword("SHOW_MAP_GRID");
            }
        }
    }

    bool show_space_grid;
    bool show_map_grid;
}