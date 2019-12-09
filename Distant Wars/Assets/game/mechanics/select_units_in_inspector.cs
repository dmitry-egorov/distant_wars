#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class select_units_in_inspector : MassiveMechanic
{
    public void _()
    {
        var su = LocalPlayer.Instance.SelectedUnits;
        if (su.Count != 0)
        {
            Selection.objects = su.Select(x => x.gameObject).ToArray();
        }
    }   

    private GameObject[] empty = new GameObject[0];
}

#endif