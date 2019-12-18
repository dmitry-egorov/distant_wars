using UnityEngine;

internal class toggle_target_prediction : MassiveMechanic
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