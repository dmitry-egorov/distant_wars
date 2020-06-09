using UnityEngine;

internal class pause_on_key : IMassiveMechanic
{
    public void _()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Debug.Break();
    }
}