#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

internal class select_units_in_inspector : MassiveMechanic
{
    public void _()
    {
        var su = LocalPlayer.Instance.SelectedUnits;
        if (su.Count == 1)
        {
            Selection.activeGameObject = su[0].gameObject;
        }
    }   

    private GameObject[] empty = new GameObject[0];
}

#endif