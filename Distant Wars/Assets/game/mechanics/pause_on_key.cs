using UnityEngine;

internal class pause_on_key : MassiveMechanic
{
    public void _()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Break();
    }
}