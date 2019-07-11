using Plugins.Lanski.Behaviours;
using UnityEngine;

internal class initialize_units_registry : MassiveMechanic
{
    public void _()
    {
        var /* units registry */ ur = UnitsRegistry.Instance;
        var  /* mesh renderer */ mr = ur.RequireComponent<MeshRenderer>();
        var /* material */ mat = mr.sharedMaterial;
        if (mat == null)
        {
            mat = new Material(Shader.Find("DW/Unit Singleton"));
            mat.SetTexture("_MainTex", ur.Texture);
            mat.SetColor("_Color", ur.Color);
            mr.sharedMaterial = mat;
        }

        ur.Material = mat;
        ur.MeshRenderer = mr;

        var mf = ur.RequireComponent<MeshFilter>();
        var msh = mf.sharedMesh;

        if (msh == null) 
            msh = mf.sharedMesh = new Mesh {name = "units"};
        
        var map = Map.Instance;
        msh.bounds = new Bounds(Vector3.zero, new Vector3(map.Scale, map.Scale, 1));
        msh.MarkDynamic();

        ur.Mesh = msh;
    }
}