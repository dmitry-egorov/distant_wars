using UnityEngine;

internal class pause_on_p : MassiveMechanic
{
    public void _()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Break();
    }
}