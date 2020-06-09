using UnityEngine;

internal class change_grid_size_on_key : IMassiveMechanic
{
    public void _()
    {
        /* units' registry */ var ur  = UnitsRegistry.Instance;

        if (Input.GetKeyDown(KeyCode.Period))
        {
            ur.SpaceGridHeight += 1;
            ur.SpaceGridWidth  += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            ur.SpaceGridHeight -= 1;
            ur.SpaceGridWidth  -= 1;
        }
    }
}