using UnityEngine;

internal class toggle_target_prediction : IMassiveMechanic
{
    public void _()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var ur = UnitsRegistry.Instance;
            ur.PredictTargetPosition = !ur.PredictTargetPosition;
        }    
    }
}